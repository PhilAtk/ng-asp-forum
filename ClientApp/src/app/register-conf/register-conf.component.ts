import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { URL_API_REG_CONF } from '../urls';

interface RegisterConfData {
	token: string | null;
}

@Component({
	selector: 'app-register-conf',
	templateUrl: './register-conf.component.html',
	styleUrls: ['./register-conf.component.css']
})
export class RegisterConfComponent {

	_http: HttpClient;
	_baseUrl: string;

	data = {} as RegisterConfData;

	constructor(route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string, http: HttpClient,) {
		this.data.token = route.snapshot.paramMap.get('token');
		this._baseUrl = baseUrl;

		this._http = http;
	}

	ngOnInit() {
		if (this.data.token && this.data.token != "") {
			this.sendConfirmation();
		}
	}

	sendConfirmation() {
		this._http.post<RegisterConfData>(this._baseUrl + URL_API_REG_CONF, this.data).subscribe({
			next: () => {
				window.alert("Registration Complete. Please proceed to user login");
				window.location.href = this._baseUrl;
			},
			error: (e) => { window.alert("Unable to register. Please contact an admin if you believe this is in error");}
		});
	}
}
