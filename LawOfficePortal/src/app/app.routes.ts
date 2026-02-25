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
		loadComponent: () => import('./pages/clients/clients.page').then((m) => m.ClientsPageComponent)
	},
	{
		path: 'clients/create',
		loadComponent: () => import('./pages/clients/client-create.page').then((m) => m.ClientCreatePageComponent)
	},
	{
		path: 'clients/:id',
		loadComponent: () => import('./pages/clients/client-update.page').then((m) => m.ClientUpdatePageComponent)
	},
	{
		path: 'opposing-parties',
		loadComponent: () => import('./pages/opposing-parties/opposing-parties.page').then((m) => m.OpposingPartiesPageComponent)
	},
	{
		path: 'opposing-parties/create',
		loadComponent: () => import('./pages/opposing-parties/opposing-party-create.page').then((m) => m.OpposingPartyCreatePageComponent)
	},
	{
		path: 'opposing-parties/:id',
		loadComponent: () => import('./pages/opposing-parties/opposing-party-update.page').then((m) => m.OpposingPartyUpdatePageComponent)
	},
	{
		path: 'cases/create',
		loadComponent: () => import('./pages/cases/case-create.page').then((m) => m.CaseCreatePageComponent)
	},
	{
		path: 'cases/:id',
		loadComponent: () => import('./pages/cases/case-update.page').then((m) => m.CaseUpdatePageComponent)
	},
	{
		path: 'cases',
		loadComponent: () => import('./pages/cases/cases.page').then((m) => m.CasesPageComponent)
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
