import { Inject, Injectable } from '@angular/core';
import { LoginResult, User } from './model';

// Cookie String Names
const CSTRING_AUTH = "auth";
const CSTRING_USERNAME = "username";
const CSTRING_USERID = "userID";
const CSTRING_ROLE = "userRole";
const CSTRING_STATE = "userState";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

	_baseUrl: string;
	_loggedIn: boolean;
	user = {} as User;
	
	constructor(@Inject('BASE_URL') baseUrl: string) {

		this._baseUrl = baseUrl;

		this.user.userName = this.getCookie(CSTRING_USERNAME);

		if (this.user.userName && this.user.userName != "") {
			this._loggedIn = true;
			this.user.userID = Number(this.getCookie(CSTRING_USERID));
			this.user.userRole = Number(this.getCookie(CSTRING_ROLE));
			this.user.userState = Number(this.getCookie(CSTRING_STATE))
		}

		else {
			this._loggedIn = false;
		}
	}

	setLogin(loginResult: LoginResult) {
		this.setCookie(CSTRING_AUTH, loginResult.token);
		this.setCookie(CSTRING_USERNAME, loginResult.userName);
		this.setCookie(CSTRING_USERID, loginResult.userID.toString());
		this.setCookie(CSTRING_ROLE, loginResult.userRole.toString());
		this.setCookie(CSTRING_STATE, loginResult.userState.toString());

		this.user.userID = loginResult.userID;
		this.user.userName = loginResult.userName;
		this.user.userRole = loginResult.userRole;
		this.user.userState = loginResult.userState;

		window.location.href = this._baseUrl;
	}

	public Logout() {
		this.deleteCookie(CSTRING_AUTH);
		this.deleteCookie(CSTRING_USERNAME);
		this.deleteCookie(CSTRING_USERID);
		this.deleteCookie(CSTRING_ROLE);
		this.deleteCookie(CSTRING_STATE);

		window.location.href = this._baseUrl;
	}

	setCookie(cname: string, cvalue: string) {
		// Default to a year expiry
		const d = new Date();
		d.setTime(d.getTime() + (365 * 24*60*60*1000));
		let expires = "expires=" + d.toUTCString();

		let cookie = cname+'='+cvalue +";"+ expires;

		document.cookie = cookie;
	}

	getCookie(cname: string) {
		let cparts = document.cookie.split('; ');

		let cookies: string[][] = [];

		cparts.forEach(part => {
			let cook = part.split('=');
			cookies.push(cook);
		})

		let cookie = cookies.find(cook => cook[0] == cname)

		if (cookie) {
			return cookie[1];
		}

		return "";
	}

	deleteCookie(cname: string) {
		let expires = 'expires=Thu, 01 Jan 1970 00:00:00 GMT;'

		let cookie = cname+'=;' + expires;

		document.cookie = cookie;
	}
}
