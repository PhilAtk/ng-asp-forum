import { Component, Inject, Input } from '@angular/core';
import { User } from 'src/app/model';
import { HttpClient } from '@angular/common/http';
import { URL_API_USER_ACTION_UNBAN } from 'src/app/urls';

@Component({
	selector: 'app-unban-button',
	templateUrl: './unban-button.component.html',
	styleUrls: ['./unban-button.component.css']
})
export class UnbanButtonComponent {

	@Input() user!: User;

	_http: HttpClient;
	_baseUrl: string;

	_editMode: boolean;

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._http = http;
		this._baseUrl = baseUrl;
		this._editMode = false;
	}

	public prepareUnban() {
		this._editMode = true;
	}

	public cancelUnban() {
		this._editMode = false;
	}

	public saveUnban() {
		this._http.post(this._baseUrl + URL_API_USER_ACTION_UNBAN + this.user.userID, null).subscribe({
			next: () => {
				window.alert("Unbanned user '" + this.user.userName + "'");
				location.reload();
			},
			error: (e) => {window.alert("Something went wrong");}
		});
	}
}
