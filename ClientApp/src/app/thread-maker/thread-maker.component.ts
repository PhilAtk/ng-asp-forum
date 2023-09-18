import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../auth.service';
import { Thread, userState } from '../model';
import { URL_API_THREAD } from '../urls';

interface ThreadCreateData {
	topic: string;
	text: string;
}

@Component({
  selector: 'app-thread-maker',
  templateUrl: './thread-maker.component.html',
  styleUrls: ['./thread-maker.component.css']
})
export class ThreadMakerComponent {

	_baseUrl: string;
	_http: HttpClient;
	_auth: AuthService;

	data = {} as ThreadCreateData;

	_canPost: Boolean;

	constructor (cookieSerivce: AuthService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._baseUrl = baseUrl;
		this._http = http;
		this._auth = cookieSerivce;

		this._canPost = false;
	}
	
	ngOnInit() {
		if (this._auth.user.userState >= userState.ACTIVE) {
			this._canPost = true;
		}
	}

	createThread() {
		console.log(this.data);

		this._http.post<Thread>(this._baseUrl + URL_API_THREAD, this.data).subscribe({
			next: (res) => {
				window.location.href = this._baseUrl + "thread/" + res.threadID;
			},
			error: (e) => { window.alert("Couldn't create thread");}
		});
	}

}
