export enum MessageType {
    Text = 0,
    MethodInvocation = 1,
    ConnectionEvent = 2
}

export class Message {
    public messageType: MessageType;
    public data: string;
}

export class InvocationDescriptor {
    public methodName: string;
    public arguments: Array<any>;
    public token: string;

    constructor(methodName: string, args: any[], token: string) {
        this.methodName = methodName;
        this.arguments = args;
        this.token = token;
    }
}
