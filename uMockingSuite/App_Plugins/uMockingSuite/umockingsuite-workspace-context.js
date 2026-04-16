import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/document';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

export class UMockingSuiteWorkspaceContext extends UmbControllerBase {
    #notificationContext;
    #documentContext;
    #authContext;
    #wasSubmitting = false;

    constructor(host) {
        super(host);

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
            this.#notificationContext = context;
        });

        this.consumeContext(UMB_AUTH_CONTEXT, (context) => {
            this.#authContext = context;
        });

        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {
            this.#documentContext = context;
            this.observe(context.isSubmitting, (isSubmitting) => {
                if (this.#wasSubmitting && !isSubmitting) {
                    this.#handleSaveCompleted();
                }
                this.#wasSubmitting = isSubmitting;
            });
        });
    }

    async #handleSaveCompleted() {
        try {
            const name = this.#documentContext?.getName() || 'this content';
            const contentTypeRaw = this.#documentContext?.getContentType();
            const contentTypeAlias =
                (contentTypeRaw && typeof contentTypeRaw === 'object' ? contentTypeRaw.alias : contentTypeRaw) ||
                'unknown';

            const token = await this.#authContext?.getLatestToken();
            if (!token) {
                console.debug('[uMockingSuite] No auth token available — skipping mocking message fetch.');
                return;
            }

            const params = new URLSearchParams({ contentName: name, contentTypeAlias });
            const response = await fetch(
                `/umbraco/management/api/v1/umockingsuite/mocking-message?${params}`,
                { headers: { Authorization: `Bearer ${token}` } }
            );

            if (!response.ok) {
                console.debug('[uMockingSuite] API returned', response.status, '— skipping toast.');
                return;
            }

            const data = await response.json();

            if (data?.message) {
                console.debug('[uMockingSuite] Showing toast:', data.message);
                this.#notificationContext?.peek('warning', {
                    data: {
                        headline: '🎭 uMockingSuite says:',
                        message: data.message
                    }
                });
            }
        } catch (error) {
            console.debug('[uMockingSuite] Failed to fetch mocking message:', error);
        }
    }
}

export default UMockingSuiteWorkspaceContext;
