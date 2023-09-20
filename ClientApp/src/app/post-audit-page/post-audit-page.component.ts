import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Post, PostAudit, postActionMap } from 'src/app/model';
import { URL_API_POST_AUDIT } from 'src/app/urls';

interface PostAuditReponse {
	post: Post,
	audits: PostAudit[];
}

@Component({
	selector: 'app-post-audit-page',
	templateUrl: './post-audit-page.component.html',
	styleUrls: ['./post-audit-page.component.css']
})
export class PostAuditPageComponent {
	_http: HttpClient;
	_baseUrl: string;

	post = {} as Post;
	audits!: PostAudit[];

	postActionMap = postActionMap;
	
	constructor(http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.post.postID = Number(route.snapshot.paramMap.get('id'));

		this._baseUrl = baseUrl;
		this._http = http;
	}

	ngOnInit() {
		this._http.get<PostAuditReponse>(this._baseUrl + URL_API_POST_AUDIT + this.post.postID).subscribe({
			next: (data) => {
				this.post = data.post;
				this.audits = data.audits;
			},
			error: (e) => { console.log(e);}
		});
	}
}
