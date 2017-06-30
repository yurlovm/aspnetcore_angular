import { NgModule }                    from '@angular/core';
import { RouterModule, Routes }        from '@angular/router';
import { ComposeMessageComponent }     from './shared/compose-message/compose-message.component';
import { NotFoundComponent }           from './shared/not-found.component';
import { AuthGuard }                   from './core/services/auth-guard.service';
import { SelectivePreloadingStrategy } from './core/services/selective-preload-strategy';

const appRoutes: Routes = [
  {
    path: 'compose',
    component: ComposeMessageComponent,
    outlet: 'popup',
    data: {
      title: 'Compose',
    }
  },
  {
    path: 'admin',
    loadChildren: './admin/admin.module#AdminModule',
    canLoad: [AuthGuard],
    data: {
      title: 'Admin',
    }
  },
  {
    path: 'crisis-center',
    loadChildren: './crisis-center/crisis-center.module#CrisisCenterModule',
    canActivate: [AuthGuard],
    data: {
      preload: true,
      title: 'crisis-center'
    }
  },
  { path: '',   redirectTo: '/heroes', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes,
      { preloadingStrategy: SelectivePreloadingStrategy }
    )
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
