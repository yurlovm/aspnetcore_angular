import { Observable } from 'rxjs/Observable';
import { Response }   from '@angular/http';

export const handleError = (error: Response | any): Observable<any> => {
  let errMsg: string;
  if (error instanceof Response) {
    errMsg = `${error.status} - ${error.statusText || ''}`;
  } else {
    errMsg = error.message ? error.message : error.toString();
  }
  // console.error(errMsg);
  return Observable.throw(errMsg);
};
