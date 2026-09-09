#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

extern "C" {
    // Get current tracking authorization status
    int AppTrackingTransparencyStatus() {
        if (@available(iOS 14, *)) {
            return (int)[ATTrackingManager trackingAuthorizationStatus];
        }
        return 3; // Authorized for iOS < 14
    }

    // Request tracking permission
    void RequestAppTrackingTransparency() {
        if (@available(iOS 14, *)) {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                NSLog(@"ATT Permission Status: %lu", (unsigned long)status);
            }];
        }
    }
}
