import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { URL_API_REG_REQ } from '../urls';

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

		this._http.post<RegisterData>(this._baseUrl + URL_API_REG_REQ, this.data).subscribe({
			next: () => {
				window.alert("Registered user: " + this.data.username);
				window.location.href = this._baseUrl + "/register/confirm";
				
			},
			error: (e) => { window.alert("Couldn't register user: " + e);}
		});
	}
}
