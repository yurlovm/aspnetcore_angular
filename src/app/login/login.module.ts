import { NgModule }             from '@angular/core';
import { LoginComponent }       from './login.component';
import { SharedModule }         from '../shared/shared.module';
import { LoginRoutingModule }   from './login-routing.module';
import { ReactiveFormsModule }  from '@angular/forms';

@NgModule({
  imports: [SharedModule, LoginRoutingModule, ReactiveFormsModule],
  exports: [],
  declarations: [LoginComponent],
  providers: [],
})
export class LoginModule { }

