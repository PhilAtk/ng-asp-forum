import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

interface ResetData {
	password: string;
	token: string | null;
}

@Component({
	selector: 'app-reset-page',
	templateUrl: './reset-page.component.html',
	styleUrls: ['./reset-page.component.css']
})
export class ResetPageComponent {

	_http: HttpClient;
	_baseUrl: string;

	data = {} as ResetData;
	passConfirm: string;

	constructor(route: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this.data.token = route.snapshot.paramMap.get('token');

		this._http = http;
		this._baseUrl = baseUrl;

		this.passConfirm = "";
	}

	public reset() {
		if (this.data.password != this.passConfirm) {
			window.alert("Password confirmation does not match");
			return;
		}

		this._http.post<ResetData>(this._baseUrl + 'api/password/reset', this.data).subscribe({
			next: () => {
				window.alert("Successfully Reset Password");
			},
			error: (e) => { window.alert("Couldn't reset password: " + e);}
		});
	}
}
