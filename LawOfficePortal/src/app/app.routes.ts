import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';

export const routes: Routes = [
	{
		path: '',
		pathMatch: 'full',
		redirectTo: 'home'
	},
	{
		path: 'home',
		loadComponent: () => import('./pages/home/home.page').then((m) => m.HomePageComponent)
	},
	{
		path: 'clients',
		loadComponent: () => import('./pages/clients/clients.page').then((m) => m.ClientsPageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'clients/create',
		loadComponent: () => import('./pages/clients/client-create.page').then((m) => m.ClientCreatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'clients/:id',
		loadComponent: () => import('./pages/clients/client-update.page').then((m) => m.ClientUpdatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'opposing-parties',
		loadComponent: () => import('./pages/opposing-parties/opposing-parties.page').then((m) => m.OpposingPartiesPageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'opposing-parties/create',
		loadComponent: () => import('./pages/opposing-parties/opposing-party-create.page').then((m) => m.OpposingPartyCreatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'opposing-parties/:id',
		loadComponent: () => import('./pages/opposing-parties/opposing-party-update.page').then((m) => m.OpposingPartyUpdatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'cases/create',
		loadComponent: () => import('./pages/cases/case-create.page').then((m) => m.CaseCreatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'cases/:id',
		loadComponent: () => import('./pages/cases/case-update.page').then((m) => m.CaseUpdatePageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'cases',
		loadComponent: () => import('./pages/cases/cases.page').then((m) => m.CasesPageComponent),
    	canActivate: [MsalGuard] 
	},
	{
		path: 'office',
		loadComponent: () => import('./pages/office/office.page').then((m) => m.OfficePageComponent), 
    	canActivate: [MsalGuard] 
	},
	{
		path: '**',
		redirectTo: 'home'
	}
];
