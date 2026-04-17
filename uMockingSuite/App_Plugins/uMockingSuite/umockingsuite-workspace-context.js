import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/document';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

console.log('[uMockingSuite] workspace-context module loaded ✅');

export class UMockingSuiteWorkspaceContext extends UmbControllerBase {
    #notificationContext;
    #documentContext;
    #authContext;
    #isFetchingMockMessage = false;

    constructor(host) {
        super(host);
        console.log('[uMockingSuite] constructor called');

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
            this.#notificationContext = context;
        });

        this.consumeContext(UMB_AUTH_CONTEXT, (context) => {
            this.#authContext = context;
        });

        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {
            console.log('[uMockingSuite] ✅ Got document workspace context');
            this.#documentContext = context;

            // Hook validateVariantsAndSubmit — confirmed as the save entry point in Umbraco 17.
            // We intercept the resolveFn callback (2nd argument) to detect a successful save.
            // IMPORTANT: wrapper must be synchronous (not async) — these are callback-based, not Promise-based.
            if (typeof context.validateVariantsAndSubmit === 'function') {
                const orig = context.validateVariantsAndSubmit.bind(context);
                context.validateVariantsAndSubmit = (variants, resolveFn, rejectFn) => {
                    const wrappedResolve = (...args) => {
                        console.log('[uMockingSuite] 🎭 Save succeeded — fetching mocking message...');
                        this.#handleSaveCompleted(); // async fire-and-forget — do NOT await
                        return resolveFn?.(...args);
                    };
                    return orig(variants, wrappedResolve, rejectFn);
                };
                console.log('[uMockingSuite] ✅ validateVariantsAndSubmit hooked');
            } else {
                console.warn('[uMockingSuite] ⚠️ validateVariantsAndSubmit not found on context');
            }
        });
    }

    async #handleSaveCompleted() {
        if (this.#isFetchingMockMessage) return;
        this.#isFetchingMockMessage = true;

        try {
            const name = this.#documentContext?.getName?.() ?? 'this content';

            const rawData = this.#documentContext?.getData?.();
            const persistedData = this.#documentContext?.getPersistedData?.();

            const contentTypeAlias =
                rawData?.contentType?.alias
                ?? rawData?.contentTypeAlias
                ?? persistedData?.contentType?.alias
                ?? persistedData?.contentTypeAlias
                ?? 'document';

            // Determine if this is a brand new content item (no persisted ID yet)
            const isNew = !persistedData?.id && !rawData?.id;

            // Extract property values for richer AI context — text-like values only, truncated
            const values = Array.isArray(rawData?.values) ? rawData.values : [];
            const propertyCount = values.length;
            const propertySample = values
                .filter(v => typeof v?.value === 'string' && v.value.trim().length > 0)
                .slice(0, 5)
                .map(v => `${v.alias}: ${v.value.trim().substring(0, 150)}`)
                .join('; ');

            console.log('[uMockingSuite] fetching mocking message — name:', name, 'type:', contentTypeAlias, 'isNew:', isNew, 'props:', propertyCount);

            const token = await this.#authContext?.getLatestToken?.();
            if (!token) {
                console.warn('[uMockingSuite] ⚠️ No auth token — skipping.');
                return;
            }

            const params = new URLSearchParams({ contentName: name, contentTypeAlias, isNew, propertyCount });
            if (propertySample) params.set('propertySample', propertySample);

            const url = `/umbraco/management/api/v1/umockingsuite/mocking-message?${params}`;
            const response = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });

            if (!response.ok) {
                console.warn('[uMockingSuite] ⚠️ API returned', response.status);
                return;
            }

            const data = await response.json();
            console.log('[uMockingSuite] 🎭 response received:', data?.headline, '|', data?.message);

            if (data?.message) {
                this.#notificationContext?.peek('warning', {
                    data: {
                        headline: data.headline ?? '🎭 uMockingSuite says:',
                        message: data.message
                    }
                });
            }
        } catch (error) {
            console.error('[uMockingSuite] ❌ Error:', error);
        } finally {
            this.#isFetchingMockMessage = false;
        }
    }
}

export default UMockingSuiteWorkspaceContext;
