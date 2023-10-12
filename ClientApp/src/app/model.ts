export interface LoginResult {
	userID: number,
	userRole: userRole,
	userName: string,
	userState: userState,
	token: string
}

export interface Post {
	postID: number,
	thread: Thread,
	author: User,
	date: number,
	dateModified: number,
	edited: boolean
	text: string
}

export enum postAction {
	CREATE,
	EDIT
}

export const postActionMap = new Map<postAction, string>([
	[postAction.CREATE, postAction[postAction.CREATE]],
	[postAction.EDIT, postAction[postAction.EDIT]]
])

export interface PostAudit {
	auditID: number,
	date: number,
	post: Post,
	action: postAction,
	info?: string,
}

export interface Thread {
	threadID: number,
	author: User,
	date: number,
	dateModified: number,
	edited: boolean,
	topic: string,
	numPosts: number
}

export interface ThreadResponse {
	thread: Thread,
	posts: Post[]
}

export enum threadAction {
	CREATE,
	EDIT
}

export const threadActionMap = new Map<threadAction, string>([
	[threadAction.CREATE, threadAction[threadAction.CREATE]],
	[threadAction.EDIT, threadAction[threadAction.EDIT]]
])

export interface ThreadAudit {
	auditID: number,
	date: number,
	thread: Thread,
	action: threadAction,
	info?: string,
}

export enum userRole {
	USER,
	VIP,
	ADMIN,
	SYSOP
}

export const roleMap = new Map<userRole, string>([
	[userRole.USER, userRole[userRole.USER]],
	[userRole.VIP, userRole[userRole.VIP]],
	[userRole.ADMIN, userRole[userRole.ADMIN]],
	[userRole.SYSOP, userRole[userRole.SYSOP]],
])

export enum userState {
	BANNED = -2,
	DISABLED = -1,
	AWAIT_REG = 0,
	ACTIVE,
}

export const stateMap = new Map<userState, string>([
	[userState.BANNED, userState[userState.BANNED]],
	[userState.DISABLED, userState[userState.DISABLED]],
	[userState.AWAIT_REG, userState[userState.AWAIT_REG]],
	[userState.ACTIVE, userState[userState.ACTIVE]]
])

export interface User {
	userID: number,
	userName: string,
	userState: userState,
	userRole: userRole,
	bio: string
}

export enum userAction {
	REGISTER,
	REGISTER_CONFIRM,
	PASS_FORGOT,
	PASS_RESET,
	BAN,
	UNBAN
}

export const userActionMap = new Map<userAction, string>([
	[userAction.REGISTER, userAction[userAction.REGISTER]],
	[userAction.REGISTER_CONFIRM, userAction[userAction.REGISTER_CONFIRM]],
	[userAction.PASS_FORGOT, userAction[userAction.PASS_FORGOT]],
	[userAction.PASS_RESET, userAction[userAction.PASS_RESET]],
	[userAction.BAN, userAction[userAction.BAN]],
	[userAction.UNBAN, userAction[userAction.UNBAN]]
])

export interface UserAudit {
	auditID: number,
	date: number,
	user: User,
	action: userAction,
	info?: string,
}