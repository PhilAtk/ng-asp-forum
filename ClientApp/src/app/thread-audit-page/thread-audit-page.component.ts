import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Thread, ThreadAudit, threadActionMap } from 'src/app/model';
import { URL_API_THREAD_AUDIT } from 'src/app/urls';

interface ThreadAuditReponse {
	thread: Thread,
	audits: ThreadAudit[];
}

@Component({
	selector: 'app-thread-audit-page',
	templateUrl: './thread-audit-page.component.html',
	styleUrls: ['./thread-audit-page.component.css']
})
export class ThreadAuditPageComponent {
	_http: HttpClient;
	_baseUrl: string;

	thread = {} as Thread;
	audits!: ThreadAudit[];

	threadActionMap = threadActionMap;
	
	constructor(http: HttpClient, route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string) {
		this.thread.threadID = Number(route.snapshot.paramMap.get('id'));

		this._baseUrl = baseUrl;
		this._http = http;
	}

	ngOnInit() {
		this._http.get<ThreadAuditReponse>(this._baseUrl + URL_API_THREAD_AUDIT + this.thread.threadID).subscribe({
			next: (data) => {
				this.thread = data.thread;
				this.audits = data.audits;
			},
			error: (e) => { console.log(e);}
		});
	}
}
