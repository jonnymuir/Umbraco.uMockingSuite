import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/document';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

console.log('[uMockingSuite] workspace-context module loaded ✅');

export class UMockingSuiteWorkspaceContext extends UmbControllerBase {
    #notificationContext;
    #documentContext;
    #authContext;
    #wasSubmitting = undefined;
    #isFetchingMockMessage = false;

    constructor(host) {
        super(host);
        console.log('[uMockingSuite] UMockingSuiteWorkspaceContext constructor called', host);

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
            console.log('[uMockingSuite] ✅ Got notification context', context);
            this.#notificationContext = context;
        });

        this.consumeContext(UMB_AUTH_CONTEXT, (context) => {
            console.log('[uMockingSuite] ✅ Got auth context', context);
            this.#authContext = context;
        });

        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {
            console.log('[uMockingSuite] ✅ Got document workspace context', context);
            this.#documentContext = context;

            // Diagnostic: log all prototype chain properties
            const protoProps = new Set();
            let p = Object.getPrototypeOf(context);
            while (p && p !== Object.prototype) {
                Object.getOwnPropertyNames(p).forEach(n => protoProps.add(n));
                p = Object.getPrototypeOf(p);
            }
            console.log('[uMockingSuite] Document context prototype properties:', [...protoProps].filter(n => !n.startsWith('#')).sort());

            // PRIMARY: wrap requestSubmit to intercept successful saves
            if (typeof context.requestSubmit === 'function') {
                const originalRequestSubmit = context.requestSubmit.bind(context);
                context.requestSubmit = async (...args) => {
                    console.log('[uMockingSuite] 🎯 requestSubmit intercepted — waiting for completion...');
                    try {
                        await originalRequestSubmit(...args);
                        console.log('[uMockingSuite] ✅ requestSubmit completed successfully — triggering handler');
                        this.#handleSaveCompleted();
                    } catch (e) {
                        console.warn('[uMockingSuite] ⚠️ requestSubmit threw:', e);
                    }
                };
            } else {
                console.warn('[uMockingSuite] ⚠️ context.requestSubmit is not a function — trying prototype:', typeof Object.getPrototypeOf(context)?.requestSubmit);
            }

            // SECONDARY (fallback): observe isSubmitting, ignoring initial undefined emission
            this.observe(context.isSubmitting, (isSubmitting) => {
                console.log('[uMockingSuite] isSubmitting changed:', isSubmitting);
                if (isSubmitting === undefined) return;
                if (this.#wasSubmitting === true && isSubmitting === false) {
                    console.log('[uMockingSuite] 🎯 isSubmitting true→false detected');
                    this.#handleSaveCompleted();
                }
                this.#wasSubmitting = isSubmitting;
            });
        });
    }

    async #handleSaveCompleted() {
        if (this.#isFetchingMockMessage) {
            console.log('[uMockingSuite] #handleSaveCompleted — already in progress, skipping');
            return;
        }
        this.#isFetchingMockMessage = true;
        console.log('[uMockingSuite] #handleSaveCompleted called');
        try {
            const name = this.#documentContext?.getName?.() || 'this content';
            const contentTypeRaw = this.#documentContext?.getContentType?.();
            console.log('[uMockingSuite] name:', name, '| contentTypeRaw:', contentTypeRaw);

            const contentTypeAlias =
                (contentTypeRaw && typeof contentTypeRaw === 'object' ? contentTypeRaw.alias : contentTypeRaw) ||
                'unknown';

            console.log('[uMockingSuite] contentTypeAlias:', contentTypeAlias);
            console.log('[uMockingSuite] authContext:', this.#authContext);

            const token = await this.#authContext?.getLatestToken?.();
            console.log('[uMockingSuite] token present:', !!token);

            if (!token) {
                console.warn('[uMockingSuite] ⚠️ No auth token available — skipping mocking message fetch.');
                return;
            }

            const params = new URLSearchParams({ contentName: name, contentTypeAlias });
            const url = `/umbraco/management/api/v1/umockingsuite/mocking-message?${params}`;
            console.log('[uMockingSuite] Fetching:', url);

            const response = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });
            console.log('[uMockingSuite] API response status:', response.status);

            if (!response.ok) {
                const body = await response.text().catch(() => '(unreadable)');
                console.warn('[uMockingSuite] ⚠️ API returned', response.status, '— body:', body);
                return;
            }

            const data = await response.json();
            console.log('[uMockingSuite] API data:', data);

            if (data?.message) {
                console.log('[uMockingSuite] 🎭 Showing toast:', data.message);
                this.#notificationContext?.peek('warning', {
                    data: {
                        headline: '🎭 uMockingSuite says:',
                        message: data.message
                    }
                });
            } else {
                console.warn('[uMockingSuite] ⚠️ No message in API response — no toast shown.');
            }
        } catch (error) {
            console.error('[uMockingSuite] ❌ Failed to fetch mocking message:', error);
        } finally {
            this.#isFetchingMockMessage = false;
        }
    }
}

export default UMockingSuiteWorkspaceContext;
