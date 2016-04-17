//
//  UnityDeviceBridge.mm
//  Unity-iPhone
//
//  Created by Masayuki Iwai on 4/17/16.
//
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface UnityDeviceBridge : NSObject

@end

@implementation UnityDeviceBridge

static UnityDeviceBridge *_sharedInstance = nil;

+ (UnityDeviceBridge *)sharedInstance {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [UnityDeviceBridge new];
    });
    return _sharedInstance;
}

@end

extern "C" {
    BOOL udb_getBatteryMonitoringEnabled();
    void udb_setBatteryMonitoringEnabled(BOOL isBatteryMonitoringEnabled);
    float udb_getBatteryLevel();
    int udb_getBatteryState();
}

BOOL udb_getBatteryMonitoringEnabled() {
    return [UIDevice currentDevice].batteryMonitoringEnabled;
}

void udb_setBatteryMonitoringEnabled(BOOL isBatteryMonitoringEnabled) {
    [UIDevice currentDevice].batteryMonitoringEnabled = isBatteryMonitoringEnabled;
}

float udb_getBatteryLevel() {
    return [UIDevice currentDevice].batteryLevel;
}

int udb_getBatteryState() {
    return (int)[UIDevice currentDevice].batteryState;
}
