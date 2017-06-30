import { Component, OnInit,
         ElementRef, ViewChild }    from '@angular/core';
import { Router } from '@angular/router';
import { AuthService }              from '../core/services/auth.service';
import { FormBuilder, FormGroup,
         Validators }               from '@angular/forms';
import { ValidationService }        from '../core/services/validation.service';

@Component({
  selector: 'my-login-form',
  templateUrl: './login.component.html',
  styleUrls: [ './login.component.css' ],
})
export class LoginComponent implements OnInit {
  public user: FormGroup;
  public isWaiting = false;
  public errMsg = '';

  @ViewChild('loginForm') public loginForm: ElementRef;

  constructor(private authService: AuthService,
              private router: Router,
              private validationService: ValidationService,
              private fb: FormBuilder) { }

  public ngOnInit() {
    this.user = this.fb.group({
        userlogin: ['', [Validators.required]],
        password: ['', [Validators.required]],
    });
  }

  public login() {
    this.isWaiting = true;
    this.errMsg = '';
    this.authService.login(this.user.value.userlogin, this.user.value.password)
      .do(() => this.isWaiting = false)
      .do(() => this.errMsg = '')
      .subscribe(
        (isLogged: boolean) => {
          if (isLogged) {
            // Get the redirect URL from our auth service If no redirect has been set, use the default
            let redirect = this.authService.redirectUrl ? this.authService.redirectUrl : '/admin';
            this.router.navigate([redirect]);
          }
        },
      (error: string) => {this.isWaiting = false; this.errMsg = 'Неверное имя пользователя или пароль'; console.log(error); });
  }

  public logout() {
    this.authService.logout();
  }

  public isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
