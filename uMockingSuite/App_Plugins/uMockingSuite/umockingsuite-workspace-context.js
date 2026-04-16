import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/document';

export class UMockingSuiteWorkspaceContext extends UmbControllerBase {
    #notificationContext;
    #documentContext;
    #wasSubmitting = false;

    constructor(host) {
        super(host);

        // Consume notification context for showing toasts
        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
            this.#notificationContext = context;
        });

        // Consume document workspace context to observe save events
        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {
            this.#documentContext = context;
            
            // Observe the isSubmitting state to detect when save completes
            // When it goes from true -> false, a save operation has finished
            this.observe(context.isSubmitting, (isSubmitting) => {
                if (this.#wasSubmitting && !isSubmitting) {
                    // Save just completed - trigger the mocking message
                    this.#handleSaveCompleted();
                }
                this.#wasSubmitting = isSubmitting;
            });
        });
    }

    async #handleSaveCompleted() {
        try {
            // Get content details from the workspace context
            const unique = this.#documentContext?.getUnique();
            const name = this.#documentContext?.getName();
            const contentType = this.#documentContext?.getContentType();
            
            if (!name || !contentType) {
                // Not enough info to fetch mocking message
                return;
            }

            // Call the uMockingSuite Management API
            const params = new URLSearchParams({
                contentName: name,
                contentTypeAlias: contentType.alias || contentType
            });
            
            const response = await fetch(`/umbraco/management/api/v1/umockingsuite/mocking-message?${params}`);
            
            if (!response.ok) {
                // Silently fail - don't disrupt the content save workflow
                return;
            }

            const data = await response.json();
            
            if (data && data.message) {
                // Show the mocking message as a warning toast
                this.#notificationContext?.peek('warning', {
                    data: {
                        headline: '🎭 uMockingSuite says:',
                        message: data.message
                    }
                });
            }
        } catch (error) {
            // Silently swallow errors - we don't want to break the save workflow
            console.debug('[uMockingSuite] Failed to fetch mocking message:', error);
        }
    }
}

export default UMockingSuiteWorkspaceContext;
