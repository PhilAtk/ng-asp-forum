import { Component } from '@angular/core';
import { CookieService } from '../cookie.service';
import { User } from '../model';

@Component({
	selector: 'app-nav-menu',
	templateUrl: './nav-menu.component.html',
	styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

	_cookieService: CookieService;
	_loggedIn: boolean;

	user = {} as User;

	constructor(cookieService: CookieService) {
		this._loggedIn = false;
		this._cookieService = cookieService;
	}

	ngOnInit() {
		this.user.userName = this._cookieService.getUsername();
		if (this.user.userName) {
			this._loggedIn = true;
			this.user.userID = this._cookieService.getUserID();
		}
	}

	public logout() {
		this._cookieService.Logout();
	}
}
