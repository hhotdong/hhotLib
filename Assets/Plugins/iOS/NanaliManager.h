//
//  MediaManager.h
//  Unity-iPhone
//
//  Created by BaeMinGi on 2017. 12. 18..
//
//

#ifndef NanaliManager_h
#define NanaliManager_h

#import <Foundation/Foundation.h>

#ifdef __cplusplus
#include <string>
class ShareManager
{
private:
    ShareManager();
public:
    ~ShareManager();
    
    static void ShareImage(const std::string& path,const std::string& message);
};

#endif //__cplusplus


#endif /* NanaliManager_h */
