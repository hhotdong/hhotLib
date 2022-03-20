@interface PhotoAlbumSave : NSObject

+(void) image:(UIImage*) image didFinishedSavingWithError:(NSError*)error contextInfo:(void*)contenxtInfo;
+(void) SaveImage:(UIImage*)image;
+(void) SuccessPopup:(NSString*)title Message:(NSString*)msg ButtonMessage:(NSString*)btn;

@end

@implementation PhotoAlbumSave

+(void) image:(UIImage*) image didFinishedSavingWithError:(NSError*)error contextInfo:(void*)contenxtInfo
{
    if(error)
    {
        [PhotoAlbumSave SuccessPopup:@"" Message:@"Please check for photo save permission in option." ButtonMessage:@"Done"];
        UnitySendMessage("CaptureUI(Clone)", "SaveCallback", "fail");
    }
    else
    {
        [PhotoAlbumSave SuccessPopup:@"" Message:@"Save success!" ButtonMessage:@"Done"];
        UnitySendMessage("CaptureUI(Clone)", "SaveCallback", "success");
    }
}

+(void) SaveImage:(UIImage*)img
{
    UIImageWriteToSavedPhotosAlbum(img, self, @selector(image:didFinishedSavingWithError:contextInfo:), nil);
}

+(void) SuccessPopup:(NSString*)title Message:(NSString*)msg ButtonMessage:(NSString*)btn
{
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:title message:msg preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* yesBtn = [UIAlertAction
                             actionWithTitle:btn
                             style:UIAlertActionStyleDefault
                             handler:^(UIAlertAction * action)
                             {
                                 
                             }];
    [alert addAction:yesBtn];
    
    // bring up the action sheet
    UIViewController *topController = [UIApplication sharedApplication].keyWindow.rootViewController;
    while (topController.presentedViewController) {
        topController = topController.presentedViewController;
    }
    
    [topController presentViewController:alert animated:YES completion:nil];
}
@end

#import "NanaliManager.h"
void ShareManager::ShareImage(const std::string& path,const std::string& message)
{
    NSString *string = [NSString stringWithUTF8String:message.c_str()];
    
    UIImage *image=[[UIImage alloc] initWithContentsOfFile:[NSString stringWithFormat:@"%s", path.c_str()]];
    
    NSArray *postItems=(image == nil)?@[string]:@[string, image];
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:postItems
                                                                                         applicationActivities:nil];
    //    NSArray *excludeActivities = @[UIActivityTypeAssignToContact, UIActivityTypePostToFacebook];
    //    activityViewController.excludedActivityTypes = excludeActivities;
    //
    UIViewController* rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    //if iPhone
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
    {
        [rootViewController presentViewController:activityViewController
                                         animated:YES
                                       completion:nil];
    }
    //if iPad
    else
    {
        // Change Rect to position Popover
        UIView *_view = rootViewController.view;
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityViewController];
        [popup presentPopoverFromRect:CGRectMake(_view.frame.size.width/2, _view.frame.size.height/4, 0, 0) inView:_view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
}

extern "C"{
    
    void RefreshGallary(const char *fileName)
    {
        NSString *file = [NSString stringWithUTF8String:(fileName)];
        UIImage *image = [UIImage imageWithContentsOfFile:file];
        // 이미지를 카메라 롤에 저장
        [PhotoAlbumSave SaveImage:image];
    }
    
    void ShareImage(char* path,char* message)
    {
        ShareManager::ShareImage(path,message);
    }

    void GetReceipt(const char *objectName, const char *callbackMethodName) //영수증 찾기.
    {
        // Get the receipt if it's available
        NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
        NSData *receipt = [NSData dataWithContentsOfURL:receiptURL];

        if (!receipt) {
            NSLog(@"-------------no receipt---------------");
            /* No local receipt -- handle the error. */
            UnitySendMessage(objectName, callbackMethodName, "");
        } else {
            /* Get the receipt in encoded format */
            NSString *encodedReceipt = [receipt base64EncodedStringWithOptions:0];
            UnitySendMessage(objectName, callbackMethodName, [encodedReceipt UTF8String]);
        }
    }
}
