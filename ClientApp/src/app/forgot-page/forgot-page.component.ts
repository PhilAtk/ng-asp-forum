import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { URL_API_PASS_FORGOT } from '../urls';

interface ForgotData {
	email: string
}

@Component({
	selector: 'app-forgot-page',
	templateUrl: './forgot-page.component.html',
	styleUrls: ['./forgot-page.component.css']
})
export class ForgotPageComponent {

	_http: HttpClient;
	_baseUrl: string;

	data = {} as ForgotData;

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._http = http;
		this._baseUrl = baseUrl;
	}

	public forgot() {
		this._http.post<ForgotData>(this._baseUrl + URL_API_PASS_FORGOT, this.data).subscribe({
			next: () => {
				window.alert("If an account exists with the given email, a password reset email was sent");
			},
			error: (e) => { window.alert("Something went wrong: " + e);}
		});
	}
}
