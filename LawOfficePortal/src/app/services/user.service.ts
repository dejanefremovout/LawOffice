import { Injectable, inject } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { jwtDecode } from 'jwt-decode';

@Injectable({
	providedIn: 'root'
})
export class UserService {
	private readonly authService = inject(MsalService);
	private readonly apiScopes = ['api://a9a5990c-f11e-49df-a582-a2c1416456cf/access_as_user'];

	async getTokenData(): Promise<Record<string, unknown> | null> {
		const account = this.authService.instance.getActiveAccount() ?? this.authService.instance.getAllAccounts()[0];
		if (!account) {
			return null;
		}

		try {
			const tokenResult = await this.authService.instance.acquireTokenSilent({
				account,
				scopes: this.apiScopes
			});

			if (!tokenResult.accessToken) {
				return null;
			}

			return jwtDecode<Record<string, unknown>>(tokenResult.accessToken);
		} catch {
			return null;
		}
	}

	async getName(): Promise<string | null> {
		return this.getStringClaim('name');
	}

	async getPreferredUsername(): Promise<string | null> {
		return this.getStringClaim('preferred_username');
	}

	async getOfficeId(): Promise<string | null> {
		return this.getStringClaim('extension_OfficeId');
	}

	private async getStringClaim(claim: string): Promise<string | null> {
		const tokenData = await this.getTokenData();
		const value = tokenData?.[claim];
		return typeof value === 'string' && value.length > 0 ? value : null;
	}
}
