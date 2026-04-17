import { LitElement, html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

console.log('[uMockingSuite] settings module loaded ✅');

class UMockingSuiteSettings extends UmbElementMixin(LitElement) {
    static styles = css`
        :host {
            display: block;
            padding: var(--uui-size-space-5);
        }

        .settings-container {
            display: flex;
            flex-direction: column;
            gap: var(--uui-size-space-5);
            max-width: 480px;
        }

        .form-field {
            display: flex;
            flex-direction: column;
            gap: var(--uui-size-space-2);
        }

        .description {
            color: var(--uui-color-text-alt);
            font-size: var(--uui-font-size-default);
            line-height: 1.5;
        }

        .status-message {
            padding: var(--uui-size-space-3);
            border-radius: var(--uui-border-radius);
            font-size: var(--uui-font-size-default);
        }

        .status-message.success {
            background-color: var(--uui-color-positive-emphasis);
            color: var(--uui-color-positive-contrast);
        }

        .status-message.error {
            background-color: var(--uui-color-danger-emphasis);
            color: var(--uui-color-danger-contrast);
        }

        .status-message.loading {
            background-color: var(--uui-color-surface-emphasis);
            color: var(--uui-color-text);
        }
    `;

    static properties = {
        _profiles: { type: Array, state: true },
        _selectedProfileAlias: { type: String, state: true },
        _loading: { type: Boolean, state: true },
        _statusMessage: { type: String, state: true },
        _statusType: { type: String, state: true }
    };

    constructor() {
        super();
        console.log('[uMockingSuite] settings constructor called');
        this._profiles = [];
        this._selectedProfileAlias = '';
        this._loading = true;
        this._statusMessage = '';
        this._statusType = '';
        this._notificationContext = null;
        this._authContext = null;
    }

    connectedCallback() {
        super.connectedCallback();
        console.log('[uMockingSuite] settings connected');

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
            this._notificationContext = context;
        });

        this.consumeContext(UMB_AUTH_CONTEXT, (context) => {
            this._authContext = context;
            this._loadData();
        });
    }

    async _loadData() {
        console.log('[uMockingSuite] loading settings and profiles...');
        this._loading = true;
        this._statusMessage = 'Loading...';
        this._statusType = 'loading';

        try {
            const token = await this._authContext?.getLatestToken?.();
            if (!token) {
                console.warn('[uMockingSuite] ⚠️ No auth token available');
                this._statusMessage = 'Authentication error';
                this._statusType = 'error';
                this._loading = false;
                return;
            }

            // Fetch profiles and current settings in parallel
            const [profilesResponse, settingsResponse] = await Promise.all([
                fetch('/umbraco/management/api/v1/umockingsuite/profiles', {
                    headers: { Authorization: `Bearer ${token}` }
                }),
                fetch('/umbraco/management/api/v1/umockingsuite/settings', {
                    headers: { Authorization: `Bearer ${token}` }
                })
            ]);

            if (!profilesResponse.ok) {
                console.warn('[uMockingSuite] ⚠️ Profiles API returned', profilesResponse.status);
                this._statusMessage = `Failed to load profiles (${profilesResponse.status})`;
                this._statusType = 'error';
                this._loading = false;
                return;
            }

            if (!settingsResponse.ok) {
                console.warn('[uMockingSuite] ⚠️ Settings API returned', settingsResponse.status);
                this._statusMessage = `Failed to load settings (${settingsResponse.status})`;
                this._statusType = 'error';
                this._loading = false;
                return;
            }

            const profiles = await profilesResponse.json();
            const settings = await settingsResponse.json();

            console.log('[uMockingSuite] loaded profiles:', profiles);
            console.log('[uMockingSuite] loaded settings:', settings);

            this._profiles = profiles;
            this._selectedProfileAlias = settings.profileAlias || '';
            this._statusMessage = '';
            this._statusType = '';
            this._loading = false;
        } catch (error) {
            console.error('[uMockingSuite] ❌ Error loading data:', error);
            this._statusMessage = `Error: ${error.message}`;
            this._statusType = 'error';
            this._loading = false;
        }
    }

    async _handleSave() {
        console.log('[uMockingSuite] saving settings...');
        this._loading = true;
        this._statusMessage = 'Saving...';
        this._statusType = 'loading';

        try {
            const token = await this._authContext?.getLatestToken?.();
            if (!token) {
                console.warn('[uMockingSuite] ⚠️ No auth token available');
                this._statusMessage = 'Authentication error';
                this._statusType = 'error';
                this._loading = false;
                return;
            }

            const response = await fetch('/umbraco/management/api/v1/umockingsuite/settings', {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ profileAlias: this._selectedProfileAlias })
            });

            if (!response.ok) {
                console.warn('[uMockingSuite] ⚠️ Save API returned', response.status);
                this._statusMessage = `Failed to save settings (${response.status})`;
                this._statusType = 'error';
                this._loading = false;
                return;
            }

            console.log('[uMockingSuite] ✅ settings saved');
            this._statusMessage = 'Settings saved successfully!';
            this._statusType = 'success';
            this._loading = false;

            // Also show a notification
            this._notificationContext?.peek('positive', {
                data: {
                    headline: 'Settings Saved',
                    message: 'uMockingSuite settings have been updated.'
                }
            });
        } catch (error) {
            console.error('[uMockingSuite] ❌ Error saving settings:', error);
            this._statusMessage = `Error: ${error.message}`;
            this._statusType = 'error';
            this._loading = false;
        }
    }

    _handleProfileChange(event) {
        this._selectedProfileAlias = event.target.value;
        console.log('[uMockingSuite] profile selected:', this._selectedProfileAlias);
    }

    render() {
        return html`
            <uui-box headline="uMockingSuite Settings">
                <div class="settings-container">
                    <p class="description">
                        Choose which AI profile uMockingSuite uses to generate mocking comments when you save content.
                    </p>

                    ${this._statusMessage ? html`
                        <div class="status-message ${this._statusType}">
                            ${this._statusMessage}
                        </div>
                    ` : ''}

                    <div class="form-field">
                        <uui-label for="profile-select" required>
                            AI Profile
                        </uui-label>
                        <uui-select
                            id="profile-select"
                            .value=${this._selectedProfileAlias}
                            @change=${this._handleProfileChange}
                            ?disabled=${this._loading}
                        >
                            ${this._profiles.map(profile => html`
                                <uui-select-option value="${profile.alias}">
                                    ${profile.name}
                                </uui-select-option>
                            `)}
                        </uui-select>
                    </div>

                    <div>
                        <uui-button
                            look="primary"
                            color="positive"
                            @click=${this._handleSave}
                            ?disabled=${this._loading || !this._selectedProfileAlias}
                        >
                            Save Settings
                        </uui-button>
                    </div>
                </div>
            </uui-box>
        `;
    }
}

customElements.define('umockingsuite-settings', UMockingSuiteSettings);

export default UMockingSuiteSettings;
