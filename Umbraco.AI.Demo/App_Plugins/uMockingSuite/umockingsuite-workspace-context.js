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

            // 1. Log ALL prototype chain properties (JSON.stringify avoids browser truncation)
            const protoProps = new Set();
            let p = Object.getPrototypeOf(context);
            while (p && p !== Object.prototype) {
                Object.getOwnPropertyNames(p).forEach(n => protoProps.add(n));
                p = Object.getPrototypeOf(p);
            }
            console.log('[uMockingSuite] PROTO PROPS:', JSON.stringify([...protoProps].sort()));

            // 2. Dynamically wrap every proto method whose name contains save/submit/handle/request/publish
            const targetPattern = /save|submit|handle|request|publish/i;
            for (const name of protoProps) {
                if (!targetPattern.test(name)) continue;
                if (typeof context[name] !== 'function') continue;
                const orig = context[name].bind(context);
                context[name] = async (...args) => {
                    console.log(`[uMockingSuite] 🔍 ${name} called`, args);
                    try {
                        const result = await orig(...args);
                        console.log(`[uMockingSuite] ✅ ${name} completed`);
                        return result;
                    } catch (e) {
                        console.warn(`[uMockingSuite] ⚠️ ${name} threw:`, e);
                        throw e;
                    }
                };
            }

            // 3. Belt-and-braces: also wrap specific names in case they're on the instance only
            const specificNames = ['requestSubmit', 'submit', '_handleSave', '_handleSubmit', 'save', 'publish'];
            for (const methodName of specificNames) {
                if (typeof context[methodName] === 'function') {
                    const orig = context[methodName].bind(context);
                    context[methodName] = async (...args) => {
                        console.log(`[uMockingSuite] 🔍 ${methodName} called`, args);
                        try {
                            const result = await orig(...args);
                            console.log(`[uMockingSuite] ✅ ${methodName} completed`);
                            return result;
                        } catch (e) {
                            console.warn(`[uMockingSuite] ⚠️ ${methodName} threw:`, e);
                            throw e;
                        }
                    };
                }
            }

            // 6. Click listener diagnostic — correlate button clicks with saves
            this._host?.addEventListener?.('click', (e) => {
                if (e.target?.tagName === 'BUTTON' || e.target?.closest?.('button')) {
                    console.log('[uMockingSuite] 🖱️ Button click:', e.target?.textContent?.trim(), e.target);
                }
            }, true);
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
