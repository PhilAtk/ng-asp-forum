import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { LoginResult, User, userRole } from './model';

const AUTH_COOKIE_STRING = "auth";
const USERNAME_COOKIE_STRING = "username";
const USERID_COOKIE_STRING = "userID";
const USERROLE_COOKIE_STRING = "userRole";
const USERSTATE_COOKIE_STRING = "userState";

@Injectable({
  providedIn: 'root'
})
export class CookieService {

	_http: HttpClient;
	_baseUrl: string;
	
	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._baseUrl = baseUrl;
		this._http = http;
	}

	setLogin(loginResult: LoginResult) {
		this.setCookie(AUTH_COOKIE_STRING, loginResult.token);
		this.setCookie(USERNAME_COOKIE_STRING, loginResult.userName);
		this.setCookie(USERID_COOKIE_STRING, loginResult.userID.toString());
		this.setCookie(USERROLE_COOKIE_STRING, loginResult.userRole.toString());
		this.setCookie(USERSTATE_COOKIE_STRING, loginResult.userState.toString());
		window.location.href = this._baseUrl;
	}

	Logout() {
		this.deleteCookie(AUTH_COOKIE_STRING);
		this.deleteCookie(USERNAME_COOKIE_STRING);
		this.deleteCookie(USERID_COOKIE_STRING);
		this.deleteCookie(USERROLE_COOKIE_STRING);
		this.deleteCookie(USERSTATE_COOKIE_STRING);
		window.location.href = this._baseUrl;
	}

	getAuth() {
		return this.getCookie(AUTH_COOKIE_STRING);
	}

	getUserRole() {
		return Number(this.getCookie(USERROLE_COOKIE_STRING));
	}

	getUserID() {
		return Number(this.getCookie(USERID_COOKIE_STRING));
	}

	getUserState() {
		return Number(this.getCookie(USERSTATE_COOKIE_STRING))
	}

	getUsername() {
		return this.getCookie(USERNAME_COOKIE_STRING);
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
		let expires = 'expires=Thu, 01 Jan 1970 00:00:00 UTC'

		let cookie = cname+'=;' + expires;

		document.cookie = cookie;
	}


}
