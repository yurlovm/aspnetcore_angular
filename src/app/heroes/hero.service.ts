import { Injectable }     from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import { Hero }           from './hero';
import { API_HEROES }     from '../shared/api';
import { handleError }    from '../shared/handle-error';

@Injectable()
export class HeroService {
  constructor (private http: Http) {}
  public getHeroes(): Observable<Hero[]> {
    return this.http.get(API_HEROES)
                    .map((res: Response) => res.json())
                    .catch(handleError);
  }

  public getHero(id: number | string): Observable<Hero> {
    return this.http.get(API_HEROES + '/' + id)
                .map((res: Response) => res.json())
                .catch(handleError);
  }
}
