import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomePageComponent } from './home-page/home-page.component';
import { ThreadListComponent } from './thread-list/thread-list.component';
import { ThreadViewComponent } from './thread-view/thread-view.component';

import { ThreadMakerComponent } from './thread-maker/thread-maker.component';
import { PostMakerComponent } from './post-maker/post-maker.component';
import { LoginBoxComponent } from './login-box/login-box.component';
import { RegisterPageComponent } from './register-page/register-page.component';
import { ResetPageComponent } from './reset-page/reset-page.component';
import { ForgotPageComponent } from './forgot-page/forgot-page.component';
import { RegisterConfComponent } from './register-conf/register-conf.component';
import { PostComponent } from './thread-view/post/post.component';
import { UserPageComponent } from './user-page/user-page.component';
import { UserListComponent } from './user-list/user-list.component';
import { BanButtonComponent } from './actions/ban-button/ban-button.component';
import { UnbanButtonComponent } from './actions/unban-button/unban-button.component';
import { UserAuditPageComponent } from './user-audit-page/user-audit-page.component';
import { ThreadAuditPageComponent } from './thread-audit-page/thread-audit-page.component';
import { PostAuditPageComponent } from './post-audit-page/post-audit-page.component';

@NgModule({
	declarations: [
		AppComponent,
		NavMenuComponent,
		HomePageComponent,
		ThreadListComponent,
		ThreadViewComponent,
		ThreadMakerComponent,
		PostMakerComponent,
		LoginBoxComponent,
		RegisterPageComponent,
		RegisterConfComponent,
		ResetPageComponent,
		ForgotPageComponent,
		PostComponent,
		UserPageComponent,
		UserListComponent,
		BanButtonComponent,
		UnbanButtonComponent,
		UserAuditPageComponent,
		ThreadAuditPageComponent,
		PostAuditPageComponent
	],
	imports: [
		BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
		HttpClientModule,
		FormsModule,
		RouterModule.forRoot([
			{ path: '', component: HomePageComponent, pathMatch: 'full' },
			{ path: 'thread/:id', component: ThreadViewComponent },
			{ path: 'thread/audit/:id', component: ThreadAuditPageComponent },
			{ path: 'post/audit/:id', component: PostAuditPageComponent},
			{ path: 'register/confirm/:token', component: RegisterConfComponent },
			{ path: 'register/confirm', component: RegisterConfComponent },
			{ path: 'register', component: RegisterPageComponent },
			{ path: 'user/:id', component: UserPageComponent },
			{ path: 'userlist', component: UserListComponent},
			{ path: 'user/audit/:id', component: UserAuditPageComponent},
			{ path: 'reset/:token', component: ResetPageComponent },
			{ path: 'forgot', component: ForgotPageComponent },
			{ path: 'login', component: LoginBoxComponent }
		], {
			anchorScrolling: 'enabled',
			onSameUrlNavigation: 'ignore'
		})
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
