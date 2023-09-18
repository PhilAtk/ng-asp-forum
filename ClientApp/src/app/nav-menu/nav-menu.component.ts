import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { User } from '../model';

@Component({
	selector: 'app-nav-menu',
	templateUrl: './nav-menu.component.html',
	styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

	_auth: AuthService;

	constructor(auth: AuthService) {
		this._auth = auth;
	}
}
