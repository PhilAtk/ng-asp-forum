import { Component, Inject, Input } from '@angular/core';
import { User } from '../model';
import { HttpClient } from '@angular/common/http';
import { URL_API_USER_ACTION_BAN } from '../urls';

interface BanData {
	reason: string,
}

@Component({
	selector: 'app-ban-button',
	templateUrl: './ban-button.component.html',
	styleUrls: ['./ban-button.component.css']
})
export class BanButtonComponent {

	@Input() user!: User;

	_http: HttpClient;
	_baseUrl: string;

	_editMode: boolean;
	data = {} as BanData;

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._http = http;
		this._baseUrl = baseUrl;
		this._editMode = false;
	}

	public prepareBan() {
		this._editMode = true;
	}

	public cancelBan() {
		this.data.reason = "";
		this._editMode = false;
	}

	public saveBan() {
		this._http.post(this._baseUrl + URL_API_USER_ACTION_BAN + this.user.userID, this.data).subscribe({
			next: () => {
				window.alert("User '" + this.user.userName + "' banned with reason: " + this.data.reason);
				location.reload();
			},
			error: (e) => {window.alert("Something went wrong");}
		});
	}
}
