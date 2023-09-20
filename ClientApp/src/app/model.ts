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

export interface Thread {
	threadID: number,
	author: User,
	date: number,
	dateModified: number,
	edited: boolean,
	topic: string,
	posts?: Post[]
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

export const actionMap = new Map<userAction, string>([
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