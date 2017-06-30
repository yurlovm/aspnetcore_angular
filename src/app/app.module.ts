import { NgModule }                from '@angular/core';
import { BrowserModule, Title }    from '@angular/platform-browser';
import { HttpModule }              from '@angular/http';
import { AppComponent }            from './app.component';
import { AppRoutingModule }        from './app-routing.module';

import { CoreModule }              from './core/core.module';
import { SharedModule }            from './shared/shared.module';
import { HeroesModule }            from './heroes/heroes.module';
import { LoginModule }             from './login/login.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  bootstrap: [ AppComponent ],
  imports: [
    BrowserModule,
    HttpModule,
    CoreModule,
    SharedModule,
    HeroesModule,
    LoginModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  declarations: [
    AppComponent
  ],
  providers: [
    Title
  ]
})
export class AppModule {}
