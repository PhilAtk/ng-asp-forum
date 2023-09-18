import { HttpClient } from '@angular/common/http';
import { Component, Inject, Input } from '@angular/core';
import { AuthService } from 'src/app/auth.service';
import { Post, userRole } from 'src/app/model';
import { URL_API_POST } from 'src/app/urls';

interface PostEditData {
	text: string,
}

@Component({
	selector: 'app-post',
	templateUrl: './post.component.html',
	styleUrls: ['./post.component.css']
})
export class PostComponent {
	@Input() post!: Post;
	
	_baseUrl: string;
	_auth: AuthService;
	_http: HttpClient;

	_canEdit: boolean;
	_editMode: boolean;

	editBuffer: string = "";

	editUrl: string = "";

	constructor(auth: AuthService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._baseUrl = baseUrl;
		this._auth = auth;
		this._http = http;

		this._canEdit = false;
		this._editMode = false;
	}

	ngOnInit() {
		if (this._auth.user.userID == this.post.author.userID || this._auth.user.userRole >= userRole.ADMIN) {
			this._canEdit = true;
		}
	}

	public deletePost() {
		this._http.delete(this._baseUrl + URL_API_POST + this.post.postID).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't delete post");}
		});
	}

	public editPost() {
		this.editBuffer = this.post.text;
		this._editMode = true;
	}

	public cancelEdit() {
		this.editBuffer = this.post.text;
		this._editMode = false;
	}

	public savePost() {
		let data = {
			text: this.editBuffer
		} as PostEditData;

		this._http.patch(this._baseUrl + URL_API_POST + this.post.postID, data).subscribe({
			next: () => {location.reload();},
			error: (e) => {window.alert("Couldn't edit post");}
		});
	}
}
