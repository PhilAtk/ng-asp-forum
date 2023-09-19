import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { userRole } from '../model';

@Component({
	selector: 'app-nav-menu',
	templateUrl: './nav-menu.component.html',
	styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

	_auth: AuthService;

	_admin: boolean;

	constructor(auth: AuthService) {
		this._auth = auth;

		this._admin = false;
		if (this._auth.user.userRole >= userRole.ADMIN) {
			this._admin = true;
		}
	}
}
