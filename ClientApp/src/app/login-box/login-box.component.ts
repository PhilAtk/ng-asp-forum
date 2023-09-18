import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { CookieService } from '../cookie.service';
import { LoginResult } from '../model';
import { URL_API_LOGIN } from '../urls';

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
	_cookieService: CookieService;
	_baseUrl: string;

	data = {} as LoginData;

	constructor(cookieService: CookieService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

		this._http = http;
		this._baseUrl = baseUrl;

		this._cookieService = cookieService;
	}

	public sendLogin() {
		this._http.post<LoginResult>(this._baseUrl + URL_API_LOGIN, this.data).subscribe({
			next: (res) => {
				this._cookieService.setLogin(res);
			},
			error: (e) => {
				window.alert("Couldn't login");
				console.log(e);
			}
		});
	}


}
