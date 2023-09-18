import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../auth.service';
import { userState } from '../model';
import { URL_API_POST } from '../urls';

interface PostCreateData {
	threadID: number;
	text: string;
}

@Component({
  selector: 'app-post-maker',
  templateUrl: './post-maker.component.html',
  styleUrls: ['./post-maker.component.css']
})
export class PostMakerComponent {

	_http: HttpClient;
	_baseUrl: string;
	_auth: AuthService;

	_canPost: boolean;

	data = {} as PostCreateData;

	constructor(cookieSerivce: AuthService, http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.data.threadID = Number(route.snapshot.paramMap.get('id'));

		this._auth = cookieSerivce;
		this._http = http;
		this._baseUrl = baseUrl;

		this._canPost = false;
	}

	ngOnInit() {
		if (this._auth.user.userState >= userState.ACTIVE) {
			this._canPost = true;
		}
	}

	public sendPost() {	
		this._http.post<PostCreateData>(this._baseUrl + URL_API_POST, this.data).subscribe({
			next: () => {location.reload();},
			error: (e) => { window.alert("Couldn't create post");}
		});
	}
}
