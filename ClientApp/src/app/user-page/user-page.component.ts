import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User, userRole } from '../model';
import { CookieService } from '../cookie.service';

interface UserEditData {
	bio: string,
}

@Component({
	selector: 'app-user-page',
	templateUrl: './user-page.component.html',
	styleUrls: ['./user-page.component.css']
})
export class UserPageComponent {

	_baseUrl: string;
	_http: HttpClient;
	_cookieService: CookieService;

	_user = {} as User;

	_canEdit: boolean;
	_editMode: boolean;

	editBuffer: string = "";

	constructor(http: HttpClient, cookieService: CookieService, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this._user.userID = Number(route.snapshot.paramMap.get('id'));

		this._http = http;
		this._cookieService = cookieService;
		this._baseUrl = baseUrl;

		this._canEdit = false;
		this._editMode = false;
	}

	ngOnInit() {
		this._http.get<User>(this._baseUrl + 'api/user/' + this._user.userID).subscribe({
			next: (u) => {
				this._user = u;
				if (this._cookieService.getUserID() == this._user.userID || this._cookieService.getUserRole() >= userRole.ADMIN) {
					this._canEdit = true;
				}
			},
			error: (e) => { console.log(e);}
		});
	}

	public editBio() {
		this.editBuffer = this._user.bio;
		this._editMode = true;
	}

	public cancelEdit() {
		this.editBuffer = this._user.bio;
		this._editMode = false;
	}

	public saveBio() {
		let data = {
			bio: this.editBuffer
		} as UserEditData;

		this._http.patch(this._baseUrl + 'api/user/' + this._user.userID, data).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't edit user profile");}
		});
	}

}
