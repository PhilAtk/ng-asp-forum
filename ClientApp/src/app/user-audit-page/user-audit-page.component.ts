import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { URL_API_USER_AUDIT } from 'src/app/urls';
import { User, UserAudit, actionMap } from 'src/app/model';

interface UserAuditReponse {
	user: User,
	audits: UserAudit[];
}

@Component({
	selector: 'app-user-audit-page',
	templateUrl: './user-audit-page.component.html',
	styleUrls: ['./user-audit-page.component.css']
})
export class UserAuditPageComponent {

	_http: HttpClient;
	_baseUrl: string;

	user = {} as User;
	audits!: UserAudit[];

	actionMap = actionMap;
	
	constructor(http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.user.userID = Number(route.snapshot.paramMap.get('id'));

		this._baseUrl = baseUrl;
		this._http = http;
	}

	ngOnInit() {
		this._http.get<UserAuditReponse>(this._baseUrl + URL_API_USER_AUDIT + this.user.userID).subscribe({
			next: (data) => {
				this.audits = data.audits;
				this.user = data.user;
			},
			error: (e) => { console.log(e);}
		});
	}
}
