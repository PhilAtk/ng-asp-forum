import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { AuthService } from 'src/app/auth.service';
import { LoginResult } from 'src/app/model';
import { URL_API_LOGIN } from 'src/app/urls';

interface LoginData {
	username: string;
	password: string;
}

@Component({
  selector: 'app-login-box',
  templateUrl: './login-box.component.html',
  styleUrls: ['./login-box.component.css']
})
export class LoginBoxComponent {

	_http: HttpClient;
	_auth: AuthService;
	_baseUrl: string;

	data = {} as LoginData;

	constructor(auth: AuthService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

		this._http = http;
		this._baseUrl = baseUrl;

		this._auth = auth;
	}

	public sendLogin() {
		this._http.post<LoginResult>(this._baseUrl + URL_API_LOGIN, this.data).subscribe({
			next: (res) => {
				this._auth.setLogin(res);
			},
			error: (e) => {
				window.alert("Couldn't login");
				console.log(e);
			}
		});
	}


}
