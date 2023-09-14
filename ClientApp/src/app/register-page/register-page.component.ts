import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { CookieService } from '../cookie.service';

interface RegisterData {
	username: string;
	password: string;
	email: string;
}

@Component({
	selector: 'app-register-page',
	templateUrl: './register-page.component.html',
	styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent {

	_http: HttpClient;
	_baseUrl: string;

	data = {} as RegisterData;
	passConfirm: string;

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._http = http;
		this._baseUrl = baseUrl;

		this.passConfirm = "";
	}

	public register() {
		if (this.data.password != this.passConfirm) {
			window.alert("Password confirmation does not match");
			return;
		}

		this._http.post<RegisterData>(this._baseUrl + '	api/register/request', this.data).subscribe({
			next: () => {
				window.alert("Registered user: " + this.data.username);
				// TODO: Redirect to confirmation page
				window.location.href = this._baseUrl;
				
			},
			error: (e) => { window.alert("Couldn't register user: " + e);}
		});
	}
}