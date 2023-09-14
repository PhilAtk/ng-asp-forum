import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Thread, userRole } from 'src/app/model';
import { CookieService } from '../cookie.service';

interface ThreadEditData {
	topic: string,
}

@Component({
  selector: 'app-thread-view',
  templateUrl: './thread-view.component.html',
  styleUrls: ['./thread-view.component.css']
})
export class ThreadViewComponent {

	_baseUrl: string;
	_http: HttpClient;
	_cookieService: CookieService;

	thread = {
		author : {},
	} as Thread;

	_canEdit: boolean;
	_editMode: boolean;

	editBuffer: string = "";

	constructor(cookieService: CookieService, http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.thread.threadID = Number(route.snapshot.paramMap.get('id'));

		this._cookieService = cookieService;

		this._baseUrl = baseUrl;
		this._http = http;

		this._canEdit = false;
		this._editMode = false;
	}

	ngOnInit() {
		this._http.get<Thread>(this._baseUrl + 'api/thread/' + this.thread.threadID).subscribe({
			next: (t) => {
				this.thread = t;
				if (this._cookieService.getUserID() == this.thread.author.userID || this._cookieService.getUserRole() >= userRole.ADMIN) {
					this._canEdit = true;
				}
			},
			error: (e) => { console.log(e);}
		});
	}

	public editThread() {
		this.editBuffer = this.thread.topic;
		this._editMode = true;
	}

	public deleteThread() {
		this._http.delete(this._baseUrl + 'api/thread/' + this.thread.threadID).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't delete thread");}
		});
	}

	public saveThread() {
		let data = {
			topic: this.editBuffer
		} as ThreadEditData;

		this._http.patch(this._baseUrl + 'api/thread/' + this.thread.threadID, data).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't edit thread");}
		});
	}

	public cancelEdit() {
		this.editBuffer = this.thread.topic;
		this._editMode = false;
	}
}
