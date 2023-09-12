import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CookieService } from '../cookie.service';
import { userRole, userState } from '../model';

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
	_cookieService: CookieService;

	_canPost: boolean;

	data = {} as PostCreateData;

	constructor(cookieSerivce: CookieService, http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.data.threadID = Number(route.snapshot.paramMap.get('id'));

		this._cookieService = cookieSerivce;
		this._http = http;
		this._baseUrl = baseUrl;

		this._canPost = false;
	}

	ngOnInit() {
		if (this._cookieService.getUserState() >= userState.ACTIVE) {
			this._canPost = true;
		}
	}

	public sendPost() {	
		this._http.post<PostCreateData>(this._baseUrl + 'api/post', this.data).subscribe({
			next: () => {location.reload();},
			error: (e) => { window.alert("Couldn't create post");}
		});
	}
}
