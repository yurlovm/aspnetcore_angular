import { NgModule }       from '@angular/core';
import { HeroListComponent }    from './hero-list.component';
import { HeroDetailComponent }  from './hero-detail.component';
import { HeroService } from './hero.service';
import { HeroRoutingModule } from './heroes-routing.module';
import { SharedModule }         from '../shared/shared.module';

@NgModule({
  imports: [
    SharedModule,
    HeroRoutingModule
  ],
  declarations: [
    HeroListComponent,
    HeroDetailComponent
  ],
  providers: [ HeroService ]
})
export class HeroesModule {}
