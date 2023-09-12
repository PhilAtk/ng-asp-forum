import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { CookieService } from '../cookie.service';
import { LoginResult, User } from '../model';

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

	_loggedIn: boolean;

	data = {} as LoginData;
	user = {} as User;

	constructor(cookieService: CookieService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

		this._http = http;
		this._baseUrl = baseUrl;

		this._cookieService = cookieService;

		this._loggedIn = false;
	}

	ngOnInit() {
		this.user.userName = this._cookieService.getUsername();
		if (this.user.userName) {
			this._loggedIn = true;
			this.user.userID = this._cookieService.getUserID();
		}
	}

	public sendLogin() {
		this._http.post<LoginResult>(this._baseUrl + 'api/login', this.data).subscribe({
			next: (res) => {
				this._cookieService.setLogin(res);
			},
			error: (e) => {
				window.alert("Couldn't login");
				console.log(e);
			}
		});
	}

	public logout() {
		this._cookieService.Logout();
	}
}
