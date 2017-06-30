import { Injectable }                                 from '@angular/core';
import { WebSocketSubject }                           from 'rxjs/observable/dom/WebSocketSubject';
import { AuthService }                                from './auth.service';
import { WindowRefService }                           from './window-ref.service';
import { MessageType, Message, InvocationDescriptor } from '../websocket';


@Injectable()
export class WebSocketService {
  private websocket: WebSocketSubject<Object>;
  public url = 'ws://localhost/ws';
  public connectionId: string;
  public enableLogging = true;
  public clientMethods: { [s: string]: Function; } = {};

  constructor(private windowRef: WindowRefService, private authsrv: AuthService) {
    if (process.env.NODE_ENV === 'production') {
      this.enableLogging = false;
    }
    const _window = windowRef.nativeWindow;
    this.url = `ws://${_window.location.hostname}:${_window.location.port}/ws`;

    this.clientMethods['EchoCallback'] = (msg: string) => {
      console.log(`Call EchoCallback method with: ${msg}`);
    }

    this.authsrv.isLoggedInObs()
      .distinctUntilChanged()
      .filter(x => x === false)
      .filter(() => this.websocket !== void 0)
      .subscribe(() => {
        this.websocket.unsubscribe();
        console.log(`WebSocket connection closed from: ${this.url}`);
      });

    this.authsrv.isLoggedInObs()
      .distinctUntilChanged()
      .filter(x => x === true)
      .switchMap(() => {
        this.websocket = new WebSocketSubject(this.url);
        return this.websocket;
      })
      .subscribe(
        (message: Message) => {
          if (message.messageType === MessageType.Text) {
            if (this.enableLogging) {
              console.log(`Text message received. Message: ${message.data}`);
            }
          } else if (message.messageType === MessageType.MethodInvocation) {
            let invocationDescriptor: InvocationDescriptor = JSON.parse(message.data);
            this.clientMethods[invocationDescriptor.methodName].apply(this, invocationDescriptor.arguments);
          } else if (message.messageType === MessageType.ConnectionEvent) {
            this.connectionId = message.data;
            if (this.enableLogging) {
              console.log(`WebSocket connected! connectionId: ${this.connectionId}`);
            }
            this.invokeServerMethod('Echo', 'aaaa');
          }
        },
        (err: any) => console.log(`WebSocket error data: ${err}`),
        () => console.log(`WebSocket connection closed from: ${this.url}`)
      );

  }

  public invokeServerMethod(methodName: string, ...args: any[]) {
    const token = this.authsrv.getToken();
    let invocationDescriptor = new InvocationDescriptor(methodName, args, token);
    if (this.enableLogging) {
      console.log(`WebSocket invoke server method "${invocationDescriptor.methodName}" with arguments: ${invocationDescriptor.arguments}`);
    }
    this.websocket.next(JSON.stringify(invocationDescriptor));
  }

}
