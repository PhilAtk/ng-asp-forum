import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Post, Thread, ThreadResponse, userRole } from 'src/app/model';
import { AuthService } from 'src/app/auth.service';
import { URL_API_THREAD } from 'src/app/urls';

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
	_auth: AuthService;

	thread = {
		author: {},
	} as Thread;
	posts: Post[] = [];

	_canEdit: boolean;
	_editMode: boolean;

	_admin: boolean;

	editBuffer: string = "";

	constructor(auth: AuthService, http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.thread.threadID = Number(route.snapshot.paramMap.get('id'));

		this._auth = auth;

		this._baseUrl = baseUrl;
		this._http = http;

		this._canEdit = false;
		this._editMode = false;

		this._admin = false;
		if (this._auth.user.userRole >= userRole.ADMIN) {
			this._admin = true;
		}
	}

	ngOnInit() {
		this._http.get<ThreadResponse>(this._baseUrl + URL_API_THREAD + this.thread.threadID).subscribe({
			next: (res) => {
				this.thread = res.thread;
				this.posts = res.posts;
				if (this._auth.user.userID == this.thread.author.userID || this._auth.user.userRole >= userRole.ADMIN) {
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
		this._http.delete(this._baseUrl + URL_API_THREAD + this.thread.threadID).subscribe({
			next: () => {
				window.alert("Successfully deleted thread");
				window.location.href = this._baseUrl;
			},
			error: (e) => {window.alert("Couldn't delete thread");}
		});
	}

	public saveThread() {
		let data = {
			topic: this.editBuffer
		} as ThreadEditData;

		this._http.patch(this._baseUrl + URL_API_THREAD + this.thread.threadID, data).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't edit thread");}
		});
	}

	public cancelEdit() {
		this.editBuffer = this.thread.topic;
		this._editMode = false;
	}
}
