import { Component, OnInit }                      from '@angular/core';
import { Title }                                  from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute }  from '@angular/router';
import { AuthService }                            from './core/services/auth.service';
import { WebSocketService }                       from './core/services/websocket.service';

@Component({
  selector: 'my-app',
  template: `
    <h1 class="title">Angular Router</h1>
    <nav>
      <a routerLink="/crisis-center" routerLinkActive="active">Crisis Center</a>
      <a routerLink="/heroes" routerLinkActive="active">Heroes</a>
      <a routerLink="/admin" routerLinkActive="active">Admin</a>
      <a routerLink="/login" routerLinkActive="active">Login</a>
      <a [routerLink]="[{ outlets: { popup: ['compose'] } }]">Contact</a>
    </nav>
    <router-outlet></router-outlet>
    <router-outlet name="popup"></router-outlet>
  `
})
export class AppComponent implements OnInit {

    public constructor(
      private router: Router,
      private titleService: Title,
      private activatedRoute: ActivatedRoute,
      private authService: AuthService,
      private ws: WebSocketService
    ) { }

    public ngOnInit(): void {
      this.setTitle('Heroes of Angular');
      this.router.events
        .filter((event) => event instanceof NavigationEnd)
        .map(() => this.activatedRoute)
        .map((route) => {
          while (route.firstChild) { route = route.firstChild; }
          return route;
        })
        .filter((route) => route.outlet === 'primary')
        .mergeMap((route) => route.data)
        .subscribe((event) => this.setTitle(event['title']) );
    }

    public setTitle( newTitle: string) {
      this.titleService.setTitle( newTitle );
    }
}
