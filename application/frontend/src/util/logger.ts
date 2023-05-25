class Logger {
  protected name: string;

  constructor(name: string) {
    this.name = name;
  }

  logError(...data: any[]) {
    console.error(`[${this?.name ?? 'Logger'}:ERROR]: `, data);
  }
  logInfo(...data: any[]) {
    console.log(`[${this?.name ?? 'Logger'}:INFO]: `, data);
  }
  logDebug(...data: any[]) {
    if (process.env.NODE_ENV !== 'production') {
      console.log(`[${this?.name ?? 'Logger'}:DEBUG]: `, data);
    }
  }
}

export const getLogger = (name: string) => new Logger(name);
