import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { User, roleMap, stateMap, userRole, userState } from '../model';

interface AdminUserEditData {
	role: userRole;
	state: userState
};

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
		this._http.get<User[]>(this._baseUrl + 'api/user').subscribe({
			next: (users) => this.users = users,
			error: (e) => { console.log(e); this.users = []}
		});
	}

	saveUser(userIndex: number) {
		let user = this.users[userIndex];

		let data = {
			role: Number(user.userRole),
			state: Number(user.userState),
		} as AdminUserEditData;

		this._http.patch(this._baseUrl + 'api/user/admin/' + user.userID, data).subscribe({
			next: () => {
				window.alert("User " + user.userName + " successfully updated");
				location.reload();
			},
			error: (e) => {window.alert("Couldn't edit user profile");}
		});
	}
}