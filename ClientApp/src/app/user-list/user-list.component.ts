import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { User, roleMap, stateMap, userRole, userState } from 'src/app/model';
import { URL_API_USERLIST } from 'src/app/urls';

@Component({
	selector: 'app-user-list',
	templateUrl: './user-list.component.html',
	styleUrls: ['./user-list.component.css']
})
export class UserListComponent {

	_baseUrl: string;
	_http: HttpClient;

	users: User[] = [];

	roleMap = roleMap;
	stateMap = stateMap;

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._baseUrl = baseUrl;
		this._http = http;
	}

	ngOnInit() {
		this._http.get<User[]>(this._baseUrl + URL_API_USERLIST).subscribe({
			next: (users) => this.users = users,
			error: (e) => { console.log(e); this.users = []}
		});
	}
}
