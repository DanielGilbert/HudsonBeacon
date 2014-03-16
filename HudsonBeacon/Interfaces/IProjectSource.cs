/*
 * 
 * HUDSON:
 <?xml version="1.0" encoding="UTF-8"?>
 *<feed xmlns="http://www.w3.org/2005/Atom">
 * <title>Alle last builds only</title>
 * <link type="text/html" href="http://127.0.0.1:8080/" rel="alternate"/>
 * <updated>2014-02-27T20:28:13Z</updated>
 * <author>
 *  <name>Hudson Server</name>
 * </author>
 * <id>urn:uuid:903deee0-7bfa-11db-9fe1-0800200c9a66</id>
 * <entry>
 *      <title>Test #4 (Stabil)</title>
 *      <link type="text/html" href="http://127.0.0.1:8080/job/Test/4/" rel="alternate"/>
 *      <id>tag:hudson.java.net,2008:http://127.0.0.1:8080/job/Test/</id>
 *      <published>2014-02-27T20:28:13Z</published>
 *      <updated>2014-02-27T20:28:13Z</updated>
 * </entry>
 * <entry>
 *      <title>test34 #1 (Stabil)</title>
 *      <link type="text/html" href="http://127.0.0.1:8080/job/test34/1/" rel="alternate"/>
 *      <id>tag:hudson.java.net,2008:http://127.0.0.1:8080/job/test34/</id>
 *      <published>2014-02-27T20:57:44Z</published>
 *      <updated>2014-02-27T20:57:44Z</updated>
 * </entry>
 *</feed>
 */

using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HudsonBeacon.Interfaces
{
    public interface IProjectSource
    {
        List<IProject> GetProjectList(String projectSource);
    }
}
