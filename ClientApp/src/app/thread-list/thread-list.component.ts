import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Thread } from 'src/app/model';
import { URL_API_THREAD } from 'src/app/urls';

@Component({
  selector: 'app-thread-list',
  templateUrl: './thread-list.component.html',
  styleUrls: ['./thread-list.component.css']
})

export class ThreadListComponent {
	
	_baseUrl: string;
	_http: HttpClient;

	threads: Thread[] = [];

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
		this._baseUrl = baseUrl;
		this._http = http;
	}

	ngOnInit() {
		this._http.get<Thread[]>(this._baseUrl + URL_API_THREAD).subscribe({
			next: (threads) => this.threads = threads,
			error: (e) => { console.log(e); this.threads = []}
		});
	}
}
