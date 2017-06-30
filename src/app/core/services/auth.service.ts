import { Injectable }               from '@angular/core';
import { Observable }               from 'rxjs/Observable';
import { BehaviorSubject }          from 'rxjs/BehaviorSubject';
import { Http, Response, Headers }  from '@angular/http';
import { LOGIN_USER_ENDPOINT }      from '../../shared/api';
import { tokenNotExpired, JwtHelper } from 'angular2-jwt';
import { handleError }              from '../../shared/handle-error';
import { Router }                   from '@angular/router';

@Injectable()
export class AuthService {
  private jwtHelper: JwtHelper = new JwtHelper();
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.isLoggedIn());
  public redirectUrl: string; // store the URL so we can redirect after logging in

  constructor(private http: Http, private router: Router) {

    Observable.interval(5000).map(() => tokenNotExpired()).filter(x => x === false).subscribe(x => this.isLoggedInSubject.next(x));

    this.isLoggedInSubject.subscribe((isLoggedIn: boolean) => {
      if (!isLoggedIn && this.getToken()) {
        localStorage.removeItem('token');
        this.router.navigate(['/login']);
      }
    });
  }

  public login(username: string, password: string): Observable<any> {
    return this.authRequest(username, password, LOGIN_USER_ENDPOINT);
  }

  public logout() {
    this.isLoggedInSubject.next(false);
  }

  public isLoggedIn() {
    return tokenNotExpired();
  }

  public isLoggedInObs(): Observable<boolean> {
    return this.isLoggedInSubject.asObservable().share();
  }

  public getToken(): string {
    return localStorage.getItem('token');
  }

  private authRequest(username: string, password: string, url: string): Observable<boolean> {
    let headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
    let body = `username=${username}&password=${password}&grant_type=password`;

    return this.http.post(url, body, { headers })
                    .map((res: Response) => res.json())
                    .do((data) => {
                      localStorage.setItem('token', data.access_token);
                      let user = this.jwtHelper.decodeToken(data.access_token);
                    })
                    .do((data) => {
                      this.isLoggedInSubject.next(true);
                    })
                    .catch(handleError);
  }

}
